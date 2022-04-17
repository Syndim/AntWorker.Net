using System.CommandLine;
using System.CommandLine.Binding;

namespace AntWorker.Net.Obsidian;
internal class ObsidianOptions : ICommandProperty
{
    public class OptionsBinder : BinderBase<ObsidianOptions>
    {
        private IList<Option<string?>> _options;

        public OptionsBinder(IList<Option<string?>> options)
        {
            _options = options;
        }

        protected override ObsidianOptions GetBoundValue(BindingContext bindingContext)
        {
            return new ObsidianOptions
            {
                SourceDir = bindingContext.ParseResult.GetValueForOption<string?>(_options[0]!),
                TargetDir = bindingContext.ParseResult.GetValueForOption<string?>(_options[1]!)
            };
        }
    }

    public string? SourceDir { get; set; }

    public string? TargetDir { get; set; }

    public IList<Option<string?>> Options { get; } = new List<Option<string?>>
        {
            new Option<string?>(new string[] { "-s", "--source-dir"}, "Source directory")
            {
                IsRequired = true,
            },
            new Option<string?>(new string[] { "-t", "--target-dir"}, "Target directory")
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

internal class ObsidianVerb : ICommandProperty
{
    internal class Decrypt : ICommandProperty
    {
        public void AddToCommand(Command c)
        {
            var decryptCommand = new Command("decrypt", "Decrypt vault");
            var keepassOptions = new KeepassOptions();
            keepassOptions.AddToCommand(decryptCommand);
            var obsidianOptions = new ObsidianOptions();
            obsidianOptions.AddToCommand(decryptCommand);
            decryptCommand.SetHandler(async (KeepassOptions keepassOptions, ObsidianOptions options) =>
            {
                await Runner.DecryptVaultAsync(keepassOptions, options);
            }, keepassOptions.CreateBinder(), obsidianOptions.CreateBinder());

            c.AddCommand(decryptCommand);
        }
    }

    public void AddToCommand(Command c)
    {
        var command = new Command("obsidian", "obsidian related commands");
        new Decrypt().AddToCommand(command);
        c.AddCommand(command);
    }
}