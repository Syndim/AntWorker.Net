using System.Diagnostics;

namespace AntWorker.Net.Git
{
    internal class Runner

    {
        public static async Task CommitAndPushAsync(string workingDir)
        {
            Logging.LogInfo($"Start to commit and push in {workingDir}");
            using (var gitAddPs = StartProcess("git", "add -A .", workingDir))
            {
                await gitAddPs.WaitForExitAsync();
                Logging.LogInfo($"git add status: {gitAddPs.ExitCode}");
            }

            using (var gitCommitPs = StartProcess("git", $"commit -m \"{DateTime.Now.ToString("u")}\"", workingDir))
            {
                await gitCommitPs.WaitForExitAsync();
                Logging.LogInfo($"git commit status: {gitCommitPs.ExitCode}");
            }

            using (var gitPushPs = StartProcess("git", "push origin HEAD", workingDir))
            {
                await gitPushPs.WaitForExitAsync();
                Logging.LogInfo($"git push status: {gitPushPs.ExitCode}");
            }
            Logging.LogInfo("Done!");
        }

        private static Process StartProcess(string fileName, string args, string workingDir)
        {
            var ps = new Process();
            ps.StartInfo.UseShellExecute = false;
            ps.StartInfo.FileName = fileName;
            ps.StartInfo.Arguments = args;
            ps.StartInfo.WorkingDirectory = workingDir;
            ps.Start();
            return ps;
        }
    }
}

