using System.CommandLine;

namespace AntWorker.Net.Notion
{
    internal class NotionVerb : ICommandProperty
    {
        private static Option<string> DatabaseIdOption = new Option<string>(new string[] { "-d", "--database-id" }, "Notion database id")
        {
            IsRequired = true,
        };

        private static Option<string> SavePathOption = new Option<string>(new string[] { "-s", "--save-path" }, "Path to save the exported notes")
        {
            IsRequired = true,
        };

        private static Option<string> ProxyOption = new Option<string>(new string[] { "-x", "--proxy" }, "Proxy address");

        private Command _command = new Command("notion", "Notion related commands");

        public NotionVerb()
        {
            SetupAddDaily();
            SetupExport();
        }

        private void SetupAddDaily()
        {
            var command = new Command("add-daily", "Add daily task page");
            var keepassOptions = new KeepassOptions();
            keepassOptions.AddToCommand(command);
            command.SetHandler(async (KeepassOptions keepassOptions, string databaseId) =>
            {
                await Runner.CreateTodayTaskPageAsync(keepassOptions, databaseId);
            }, keepassOptions.CreateBinder(), DatabaseIdOption);
            command.AddOptions(DatabaseIdOption);

            _command.Add(command);
        }

        private void SetupExport()
        {
            var command = new Command("export", "Export notes");
            var keepassOptions = new KeepassOptions();
            keepassOptions.AddToCommand(command);
            command.SetHandler(async (KeepassOptions options, string savePath, string proxy) =>
            {
                await Runner.ExportAsync(options, proxy, "markdown", savePath);
                await Runner.ExportAsync(options, proxy, "html", savePath);
            }, keepassOptions.CreateBinder(), SavePathOption, ProxyOption);
            command.AddOptions(SavePathOption, ProxyOption);
            _command.Add(command);
        }

        public void AddToCommand(Command c)
        {
            c.Add(_command);
        }
    }
}
