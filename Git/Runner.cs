using System.Diagnostics;

namespace AntWorker.Net.Git;
internal static class Runner
{
    public static async Task CommitAndPushAsync(string workingDir)
    {
        Logging.LogInfo($"Start to commit and push in {workingDir}");
        using (var gitAddPs = Helper.StartProcess("git", "add -A .", workingDir))
        {
            await gitAddPs.WaitForExitAsync();
            Logging.LogInfo($"git add status: {gitAddPs.ExitCode}");
        }

        using (var gitCommitPs = Helper.StartProcess("git", $"commit -m \"{DateTime.Now.ToString("u")}\"", workingDir))
        {
            await gitCommitPs.WaitForExitAsync();
            Logging.LogInfo($"git commit status: {gitCommitPs.ExitCode}");
        }

        using (var gitPushPs = Helper.StartProcess("git", "push origin HEAD", workingDir))
        {
            await gitPushPs.WaitForExitAsync();
            Logging.LogInfo($"git push status: {gitPushPs.ExitCode}");
        }
        Logging.LogInfo("Done!");
    }
}