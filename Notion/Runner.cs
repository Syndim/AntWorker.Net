namespace AntWorker.Net.Notion
{
    internal static class Runner
    {
        public static async Task CreateTodayTaskPageAsync(KeepassOptions keepassArgs, NotionOptions args)
        {
            Keepass.Open(keepassArgs.Path!, keepassArgs.Key!);
            var token = Keepass.GetEntry(keepassArgs.Entry!)?.Password;

            var bot = new Bot(token!);

            await bot.CreateTaskPageAsync(args.DatabaseId!);
        }
    }
}
