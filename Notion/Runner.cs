namespace AntWorker.Net.Notion
{
    internal static class Runner
    {
        public static async Task CreateTodayTaskPageAsync(NotionOptions args)
        {
            Keepass.Open(args.Path!, args.Key!);
            var token = Keepass.GetPassword(args.Entry!);

            var bot = new Bot(token!);

            await bot.CreateTaskPageAsync(args.DatabaseId!);
        }
    }
}
