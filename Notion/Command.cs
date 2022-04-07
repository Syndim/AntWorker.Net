using System.CommandLine;
using System.CommandLine.Binding;

namespace AntWorker.Net.Notion
{
    internal class NotionOptions : ICommandProperty
    {
        public class OptionsBinder : BinderBase<NotionOptions>
        {
            private IList<Option<string?>> _options;

            public OptionsBinder(IList<Option<string?>> options)
            {
                _options = options;
            }

            protected override NotionOptions GetBoundValue(BindingContext bindingContext)
            {
                return new NotionOptions
                {
                    DatabaseId = bindingContext.ParseResult.GetValueForOption<string?>(_options[0]!)
                };
            }
        }

        public string? DatabaseId { get; set; }

        public IList<Option<string?>> Options { get; } = new List<Option<string?>>
        {
            new Option<string?>(new string[] { "-d", "--database-id"}, "Notion database id")
            {
                IsRequired = true,
            }
        };

        public OptionsBinder CreateBinder() => new OptionsBinder(Options);

        public void AddToCommand(Command c)
        {
            foreach (var option in Options)
            {
                c.AddOption(option);
            }
        }
    }

    internal class NotionVerb : ICommandProperty
    {
        internal class AddDaily : ICommandProperty
        {
            public void AddToCommand(Command c)
            {
                var addDailyCommand = new Command("add-daily", "Add daily task page");
                var keepassOptions = new KeepassOptions();
                keepassOptions.AddToCommand(c);
                var notionOptions = new NotionOptions();
                notionOptions.AddToCommand(c);
                addDailyCommand.SetHandler(async (KeepassOptions keepassOptions, NotionOptions options) =>
                {
                    await Runner.CreateTodayTaskPageAsync(keepassOptions, options);
                }, keepassOptions.CreateBinder(), notionOptions.CreateBinder());

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
