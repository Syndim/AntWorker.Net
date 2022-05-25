using System.CommandLine;

namespace AntWorker.Net.Adguard
{
    internal class AdguardVerb : ICommandProperty
    {
        private static Option<string> RootOption = new Option<string>(new string[] { "-r", "--root" }, "Adguard root")
        {
            IsRequired = true,
        };

        private static Option<string[]> UrlsOption = new Option<string[]>(new string[] { "-u", "--urls" }, "Rule urls")
        {
            IsRequired = true
        };

        private Command _command = new Command("adguard", "Adguard related commands");

        public AdguardVerb()
        {

        }

        private void SetupUpdateRules()
        {
            var command = new Command("update-rules", "Update rules");
            var keepassOptions = new KeepassOptions();
            keepassOptions.AddToCommand(command);
            command.SetHandler(async (KeepassOptions keepassOptions, string root, string[] urls) =>
            {
                await Runner.UpdateRulesAsync(keepassOptions, root, urls);
            }, keepassOptions.CreateBinder(), RootOption, UrlsOption);

            command.AddOptions(RootOption, UrlsOption);
            _command.Add(command);
        }

        public void AddToCommand(Command c)
        {
            c.Add(_command);
        }
    }
}
