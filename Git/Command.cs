using System.CommandLine;
using System.CommandLine.Binding;

namespace AntWorker.Net.Git
{
    internal class GitOptions : ICommandProperty
    {
        public class OptionsBinder : BinderBase<GitOptions>
        {
            private IList<Option<string?>> _options;

            public OptionsBinder(IList<Option<string?>> options)
            {
                _options = options;
            }

            protected override GitOptions GetBoundValue(BindingContext bindingContext)
            {
                return new GitOptions
                {
                    WorkingDir = bindingContext.ParseResult.GetValueForOption<string?>(_options[0]!)
                };
            }
        }

        public string? WorkingDir { get; set; }

        public IList<Option<string?>> Options { get; } = new List<Option<string?>>
        {
            new Option<string?>(new string[] { "-w", "--working-dir"}, "Directory where changes need to be committed")
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

    internal class GitVerb : ICommandProperty
    {
        internal class CommitAndPush : ICommandProperty
        {
            public void AddToCommand(Command c)
            {
                var commitAndPushCommand = new Command("commit-and-push", "Commit and push changes");
                var gitOptions = new GitOptions();
                gitOptions.AddToCommand(commitAndPushCommand);
                commitAndPushCommand.SetHandler(async (GitOptions options) =>
                {
                    await Runner.CommitAndPushAsync(options.WorkingDir!);
                }, gitOptions.CreateBinder());

                c.AddCommand(commitAndPushCommand);
            }
        }

        public void AddToCommand(Command c)
        {
            var command = new Command("git", "Git related commands");
            new CommitAndPush().AddToCommand(command);
            c.AddCommand(command);
        }
    }
}
