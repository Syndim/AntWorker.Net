using System.CommandLine;
using System.CommandLine.Binding;

namespace AntWorker.Net.Adguard
{
    internal class AdguardOptions : ICommandProperty
    {
        public class OptionsBinder : BinderBase<AdguardOptions>
        {
            private IList<Option> _options;

            public OptionsBinder(IList<Option> options)
            {
                _options = options;
            }

            protected override AdguardOptions GetBoundValue(BindingContext bindingContext)
            {
                return new AdguardOptions
                {
                    Root = bindingContext.ParseResult.GetValueForOption<string?>((_options[0] as Option<string?>)!),
                    Urls = bindingContext.ParseResult.GetValueForOption<string[]?>((_options[1] as Option<string[]?>)!)
                };
            }
        }

        public string? Root { get; set; }

        public string[]? Urls { get; set; }

        public IList<Option> Options { get; } = new List<Option>
        {
            new Option<string?>(new string[] { "-r", "--root"}, "Adguard root")
            {
                IsRequired = true,
            },
            new Option<string[]?>(new string[] { "-u", "--urls" }, "Rule urls")
            {
                IsRequired = true
            }
        };

        public void AddToCommand(Command c)
        {
            foreach (var option in Options)
            {
                c.AddOption(option);
            }
        }

        public OptionsBinder CreateBinder() => new OptionsBinder(Options);
    }

    internal class AdguardVerb : ICommandProperty
    {
        internal class UpdateRules : ICommandProperty
        {
            public void AddToCommand(Command c)
            {
                var updateRulesCommand = new Command("update-rules", "Update rules");
                var keepassOptions = new KeepassOptions();
                keepassOptions.AddToCommand(updateRulesCommand);
                var options = new AdguardOptions();
                options.AddToCommand(updateRulesCommand);
                updateRulesCommand.SetHandler(async (KeepassOptions keepassOptions, AdguardOptions options) =>
                {
                    await Runner.UpdateRulesAsync(keepassOptions, options);
                }, keepassOptions.CreateBinder(), options.CreateBinder());
                c.AddCommand(updateRulesCommand);
            }
        }

        public void AddToCommand(Command c)
        {
            var command = new Command("adguard", "Adguard related commands");
            new UpdateRules().AddToCommand(command);
            c.AddCommand(command);
        }
    }
}
