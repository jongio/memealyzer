using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Ngrok.ApiClient;
using Nito.AsyncEx;

namespace Lib.Proxy
{
    public class NgrokClient
    {
        private HttpClient http;
        private NgrokHttpClient ngrokHttpClient;

        public NgrokClient(HttpClient http)
        {
            this.http = http;
            this.ngrokHttpClient = new NgrokHttpClient(this.http);
        }

        public async Task<Uri> GetUriAsync(int port)
        {
            var tunnels = await ngrokHttpClient.ListTunnelsAsync();
            var tunnel = tunnels.FirstOrDefault(t => t.Config.Address.EndsWith(port.ToString()));
            if (tunnel != null)
            {
                return new Uri(tunnel.PublicURL);
            }

            return null;
        }

        public Uri GetUri(int port)
        {
            var task = Task.Run(() => AsyncContext.Run(() => GetUriAsync(port)));
            return task.GetAwaiter().GetResult();
        }
    }
}