using System.Diagnostics;
using System.Net;
using Flurl.Http;
using Flurl.Http.Configuration;

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

    public class ProxyHttpClientFactory : DefaultHttpClientFactory
    {
        private string _address;

        public ProxyHttpClientFactory(string address)
        {
            _address = address;
        }

        public override HttpMessageHandler CreateMessageHandler()
        {
            return new HttpClientHandler
            {
                Proxy = new WebProxy(_address),
                UseProxy = true
            };
        }
    }

    public static void SetFlurlProxy(string address)
    {
        if (string.IsNullOrEmpty(address))
        {
            return;
        }

        FlurlHttp.Configure(settings =>
        {
            settings.HttpClientFactory = new ProxyHttpClientFactory(address);
        });
    }
}