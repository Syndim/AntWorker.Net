using System.CommandLine;

namespace AntWorker.Net.Git
{
    internal class GitVerb : ICommandProperty
    {
        private Option<string> WorkingDirOption = new Option<string>(new string[] { "-w", "--working-dir" }, "Directory where changes need to be committed")
        {
            IsRequired = true,
        };

        private Command _command = new Command("git", "Git related commands");

        public GitVerb()
        {
            SetupCommitAndPush();
        }

        private void SetupCommitAndPush()
        {
            var command = new Command("commit-and-push", "Commit and push changes");
            command.SetHandler(async (string workingDir) =>
            {
                await Runner.CommitAndPushAsync(workingDir);
            }, WorkingDirOption);
            command.AddOptions(WorkingDirOption);

            _command.Add(command);
        }

        public void AddToCommand(Command c)
        {
            c.AddCommand(_command);
        }
    }
}
