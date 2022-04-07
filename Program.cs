using System.CommandLine;
using AntWorker.Net.Notion;

namespace AntWorker.Net
{
    internal static class CommandArgs
    {
        public static async Task InvokeAsync(string[] args)
        {
            var commands = new List<ICommandProperty>
            {
                new NotionVerb()
            };

            var rootCommand = new RootCommand();
            commands.ForEach(c => c.AddToCommand(rootCommand));
            await rootCommand.InvokeAsync(args);
        }
    }

    class Program
    {
        public static async Task Main(string[] args)
        {
            await CommandArgs.InvokeAsync(args);
        }
    }
}
