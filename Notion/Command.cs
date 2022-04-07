using System.CommandLine;
using System.CommandLine.Invocation;

namespace AntWorker.Net.Notion
{
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
                        await Runner.CreateTodayTaskPageAsync(options);
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
