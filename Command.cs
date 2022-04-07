using System.CommandLine;
using System.CommandLine.Invocation;

namespace AntWorker.Net
{
    interface ICommandProperty
    {
        void AddToCommand(Command c);
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
}
