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

        private static Option<string> UserIdOption = new Option<string>(new string[] { "-u", "--user-id" }, "User id")
        {
            IsRequired = true,
        };

        private static Option<string> ProxyOption = new Option<string>(new string[] { "-x", "--proxy" }, "Proxy address");

        private Command _command = new Command("notion", "Notion related commands");

        public NotionVerb()
        {
            SetupAddDaily();
            SetupExport();
            SetupArchive();
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

        private void SetupArchive()
        {
            var command = new Command("archive", "Archive completed tasks");
            var keepassOptions = new KeepassOptions();
            keepassOptions.AddToCommand(command);
            command.SetHandler(async (KeepassOptions keepassOptions, string databaseId) =>
            {
                await Runner.ArchiveAsync(keepassOptions, databaseId);
            }, keepassOptions.CreateBinder(), DatabaseIdOption);
            command.AddOptions(DatabaseIdOption);

            _command.Add(command);
        }

        private void SetupSetCompleteDate()
        {
            var command = new Command("set-complete-date", "Archive completed tasks");
            var keepassOptions = new KeepassOptions();
            keepassOptions.AddToCommand(command);
            command.SetHandler(async (KeepassOptions keepassOptions, string databaseId) =>
            {
                await Runner.SetCompleteDateAsync(keepassOptions, databaseId);
            }, keepassOptions.CreateBinder(), DatabaseIdOption);
            command.AddOptions(DatabaseIdOption);

            _command.Add(command);
        }

        private void SetupExport()
        {
            var command = new Command("export", "Export notes");
            var keepassOptions = new KeepassOptions();
            keepassOptions.AddToCommand(command);
            command.SetHandler(async (KeepassOptions options, string savePath, string proxy, string userId) =>
            {
                await Runner.ExportAsync(options, proxy, "markdown", savePath, userId);
                await Runner.ExportAsync(options, proxy, "html", savePath, userId);
            }, keepassOptions.CreateBinder(), SavePathOption, ProxyOption, UserIdOption);
            command.AddOptions(SavePathOption, ProxyOption, UserIdOption);
            _command.Add(command);
        }

        public void AddToCommand(Command c)
        {
            c.Add(_command);
        }
    }
}
