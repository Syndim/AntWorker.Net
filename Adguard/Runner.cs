using System.Text;
using Flurl;
using Flurl.Http;

namespace AntWorker.Net.Adguard
{
    internal static class Runner
    {
        public static async Task UpdateRulesAsync(KeepassOptions keepassOptions, AdguardOptions options)
        {
            Keepass.Open(keepassOptions.Path!, keepassOptions.Key!);
            var entry = Keepass.GetEntry(keepassOptions.Entry!);
            var sb = new StringBuilder();
            foreach (var url in options.Urls!)
            {
                var content = await url.GetStringAsync();
                sb.Append(content);
                sb.Append(Environment.NewLine);
            }

            await options.Root.AppendPathSegments("control", "filtering", "set_rules")
                .WithBasicAuth(entry?.Username!, entry?.Password)
                .WithHeader("Content-Type", "text/plain")
                .PostStringAsync(sb.ToString());
        }
    }
}
