using System.CommandLine;
using AntWorker.Net.Adguard;
using AntWorker.Net.Notion;
using AntWorker.Net.Git;

namespace AntWorker.Net
{
    internal static class CommandArgs
    {
        public static async Task<int> InvokeAsync(string[] args)
        {
            var commands = new List<ICommandProperty>
            {
                new NotionVerb(),
                new AdguardVerb(),
                new GitVerb(),
            };

            var rootCommand = new RootCommand();
            commands.ForEach(c => c.AddToCommand(rootCommand));
            return await rootCommand.InvokeAsync(args);
        }
    }

    class Program
    {
        public static async Task<int> Main(string[] args)
        {
            return await CommandArgs.InvokeAsync(args);
        }
    }
}
