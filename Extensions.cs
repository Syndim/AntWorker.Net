namespace AntWorker.Net;

using System.CommandLine;

public static class Extensions
{
    public static void AddOptions(this Command command, params Option[] options)
    {
        foreach (var option in options)
        {
            command.AddOption(option);
        }
    }
}
