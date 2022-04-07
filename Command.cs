using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Invocation;

namespace AntWorker.Net
{
    interface ICommandProperty
    {
        void AddToCommand(Command c);
    }

    internal class KeepassOptions : ICommandProperty
    {
        public class OptionsBinder : BinderBase<KeepassOptions>
        {
            private readonly IList<Option<string?>> _options;

            public OptionsBinder(IList<Option<string?>> options)
            {
                _options = options;
            }

            protected override KeepassOptions GetBoundValue(BindingContext bindingContext)
            {
                return new KeepassOptions
                {
                    Path = bindingContext.ParseResult.GetValueForOption<string?>(_options[0]!),
                    Key = bindingContext.ParseResult.GetValueForOption<string?>(_options[1]!),
                    Entry = bindingContext.ParseResult.GetValueForOption<string?>(_options[2]!)
                };
            }
        }

        public string? Path { get; set; }

        public string? Key { get; set; }

        public string? Entry { get; set; }

        public List<Option<string?>> Options { get; } = new List<Option<string?>>
        {
            new Option<string?>(new string[] {"-p", "--path" }, "Keepass database path")
            {
                IsRequired = true,
            },
            new Option<string?>(new string[] { "-k", "--key" }, "Keepass password")
            {
                IsRequired = true
            },
            new Option<string?>(new string[] { "-e", "--entry" }, "Item entry path")
            {
                IsRequired = true
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
}
