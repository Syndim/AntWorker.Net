namespace AntWorker.Net.Notion;
using Flurl;
using Flurl.Http;
using Newtonsoft.Json;

internal static class Runner
{
    public static async Task CreateTodayTaskPageAsync(KeepassOptions keepassArgs, string databaseId)
    {
        Keepass.Open(keepassArgs.Path!, keepassArgs.Key!);
        var token = Keepass.GetEntry(keepassArgs.Entry!)?.Password;

        var bot = new Bot(token!);

        await bot.CreateTaskPageAsync(databaseId);
    }

    class TaskInfo
    {
        [JsonProperty("taskId")]
        public string? TaskId { get; set; }
    }

    class TaskStatus
    {
        [JsonProperty("pagesExported")]
        public int Exported { get; set; }

        [JsonProperty("exportURL")]
        public string? ExportUrl { get; set; }
    }

    class TaskItem
    {
        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("status")]
        public TaskStatus? Status { get; set; }

        [JsonProperty("state")]
        public string? State { get; set; }

        [JsonProperty("error")]
        public string? Error { get; set; }
    }

    class TaskResults
    {
        [JsonProperty("results")]
        public TaskItem[]? Results { get; set; }
    }

    class Response<T>
    {
        [JsonProperty("data")]
        public T? Data { get; set; }
    }

    public static async Task ExportAsync(KeepassOptions keepassArgs, string format, string savePath)
    {
        const string API_BASE = "https://www.notion.so/api/v3";
        Keepass.Open(keepassArgs.Path!, keepassArgs.Key!);
        var entry = Keepass.GetEntry(keepassArgs.Entry!);
        Logging.LogInfo($"Start task for format {format}");
        var taskResult = await API_BASE.AppendPathSegment("enqueueTask")
            .WithCookie("token_v2", entry?.Password!)
            .PostJsonAsync(new
            {
                task = new
                {
                    eventName = "exportSpace",
                    request = new
                    {
                        spaceId = entry?.Username!,
                        exportOptions = new
                        {
                            exportType = format,
                            timeZone = "Asia/Shangha",
                            locale = "en",
                        },
                    },
                },
            })
        .ReceiveJson<TaskInfo>();
        var taskId = taskResult.TaskId!;
        Logging.LogInfo($"Task id: {taskId}");

        int failedCount = 0;

        string exportUrl = string.Empty;

        while (true && failedCount <= 5)
        {
            await Task.Delay(10000);
            var taskStatus = await API_BASE.AppendPathSegment("getTasks")
                .WithCookie("token_v2", entry?.Password!)
                .PostJsonAsync(new
                {
                    taskIds = new string[] { taskId }
                })
            .ReceiveJson<TaskResults>();
            var task = taskStatus.Results?.FirstOrDefault((t) => t.Id == taskId);
            if (task == null)
            {
                failedCount++;
                Logging.LogInfo("Failed to get task info");
                continue;
            }

            if (task.Status == null)
            {
                failedCount++;
                Logging.LogInfo("Failed to get task status");
                continue;
            }

            if (task.State == "in_progress")
            {
                Logging.LogInfo($"Pages exported: {task.Status?.Exported}");
                continue;
            }

            if (task.State == "failure")
            {
                Logging.LogInfo($"Task error: {task.Error}");
                break;
            }

            if (task.State == "success")
            {
                exportUrl = task.Status?.ExportUrl!;
                Logging.LogInfo($"Export url: {exportUrl}");
                break;
            }
        }

        var stream = await exportUrl.GetStreamAsync();
        using var outputFile = File.Create(Path.Combine(savePath, $"{format}.zip"));
        await stream.CopyToAsync(outputFile);
    }
}
