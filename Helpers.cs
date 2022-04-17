using System.Diagnostics;

namespace AntWorker.Net;

internal static class Helper
{
    public static Process StartProcess(string fileName, string args, string workingDir, bool useShell = false)
    {
        var ps = new Process();
        ps.StartInfo.UseShellExecute = useShell;
        ps.StartInfo.FileName = fileName;
        ps.StartInfo.Arguments = args;
        ps.StartInfo.WorkingDirectory = workingDir;
        if (!useShell)
        {
            ps.StartInfo.RedirectStandardOutput = true;
            ps.StartInfo.RedirectStandardInput = true;
        }

        ps.Start();
        return ps;
    }
}