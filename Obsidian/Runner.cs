using System.Text;
namespace AntWorker.Net.Obsidian;

internal class Runner
{
    public static async Task DecryptVaultAsync(KeepassOptions keepassOptions, ObsidianOptions obsidianOptions)
    {
        Keepass.Open(keepassOptions.Path!, keepassOptions.Key!);
        var password = Keepass.GetEntry(keepassOptions.Entry!)?.Password;
        await DecryptDir(password!, obsidianOptions.SourceDir!, obsidianOptions.TargetDir!, obsidianOptions.SourceDir!);
    }

    private static async Task DecryptDir(string password, string sourceRoot, string targetRoot, string current)
    {
        var subDirs = Directory.EnumerateDirectories(current);
        foreach (var dir in subDirs)
        {
            await DecryptDir(password, sourceRoot, targetRoot, Path.Combine(current, dir));
        }

        var subFiles = Directory.EnumerateFiles(current);
        foreach (var f in subFiles)
        {
            var fileName = NormalizeBase64(Path.GetFileName(f));
            var fileNameBytes = Convert.FromBase64String(fileName);
            var fileNameDecrypted = await DecryptBytes(fileNameBytes, password, current);
            var targetFilePath = Path.Combine(targetRoot, fileNameDecrypted);
            var targetFolder = Directory.GetParent(targetFilePath)?.FullName!;
            Logging.LogInfo($"{fileName} => {targetFilePath}");
            Directory.CreateDirectory(targetFolder);
            using var file = File.Open(f, FileMode.Open);
            if (file.Length == 0)
            {
                Logging.LogInfo("Skip decrypt for directory");
                continue;
            }

            await DecryptFile(f, targetFilePath, password, current);
            // var buffer = new byte[file.Length];
            // var bytesRead = await file.ReadAsync(buffer);
            // if (bytesRead != file.Length)
            // {
            //     throw new Exception("Failed to read all bytes!");
            // }

            // var contentDecrypted = await DecryptBytes(buffer, password, current);
            // using var targetFile = File.Create(targetFilePath);
            // using var sw = new StreamWriter(targetFile);
            // await sw.WriteAsync(contentDecrypted);
        }
    }

    private static string NormalizeBase64(string origin)
    {
        var target = origin.Replace("-", "+").Replace("_", "/");
        if (origin.Length % 4 == 3)
        {
            target = target + "=";
        }
        else if (origin.Length % 4 == 2)
        {
            target = target + "==";
        }

        return target;
    }

    private static async Task<string> DecryptBytes(byte[] content, string password, string currentDir)
    {
        var ps = Helper.StartProcess("openssl", $"enc -d -aes-256-cbc -pbkdf2 -iter 20000 -pass pass:{password}", currentDir);
        await ps.StandardInput.BaseStream.WriteAsync(content);
        ps.StandardInput.BaseStream.Close();
        await ps.WaitForExitAsync();
        return await ps.StandardOutput.ReadToEndAsync();
    }

    private static async Task DecryptFile(string origin, string target, string password, string currentDir)
    {
        var args = $"enc -in \"{origin}\" -out \"{target}\" -d -aes-256-cbc -pbkdf2 -iter 20000 -pass pass:{password}";
        // Logging.LogInfo($"Running openssl {args}");
        var ps = Helper.StartProcess("openssl", args, currentDir, true);
        await ps.WaitForExitAsync();
    }
}