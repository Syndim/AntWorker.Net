using System.CommandLine;
using System.CommandLine.Invocation;

namespace AntWorker.Net
{
    interface ICommandProperty
    {
        void AddToCommand(Command c);
    }

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

    internal class KeepassOptions : ICommandProperty
    {
        public string? Path { get; set; }

        public string? Key { get; set; }

        public string? Entry { get; set; }

        public virtual void AddToCommand(Command c)
        {
            var options = new List<Option>
            {
                new Option<string>(new string[] {"-p", "--path" }, "Keepass database path")
                {
                    IsRequired = true,
                },
                new Option<string>(new string[] { "-k", "--key"}, "Keepass password")
                {
                    IsRequired = true
                },
                new Option<string>(new string[]{ "-e", "--entry"}, "Item entry path")
                {
                    IsRequired = true
                }
            };

            options.ForEach(option => c.Add(option));
        }
    }

    internal class NotionOptions : KeepassOptions
    {
        public string? DatabaseId { get; set; }

        public override void AddToCommand(Command c)
        {
            base.AddToCommand(c);

            var options = new List<Option>
            {
                new Option<string>(new string[] { "-d", "--database-id"}, "Notion database id")
                {
                    IsRequired = true,  
                }
            };

            options.ForEach(option => c.AddOption(option));
        }
    }

    internal class NotionVerb : ICommandProperty
    {
        internal class AddDaily : ICommandProperty
        {
            public void AddToCommand(Command c)
            {
                var addDailyCommand = new Command("add-daily", "Add daily task page")
                {
                    Handler = CommandHandler.Create<NotionOptions>(async (options) =>
                    {
                        await Notion.Runner.CreateTodayTaskPageAsync(options);
                    })
                };

                new NotionOptions().AddToCommand(addDailyCommand);
                c.AddCommand(addDailyCommand);
            }
        }

        public void AddToCommand(Command c)
        {
            var command = new Command("notion", "Notion related commands");
            new AddDaily().AddToCommand(command);
            c.AddCommand(command);
        }
    }
}
