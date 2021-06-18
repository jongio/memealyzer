using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Ngrok.ApiClient;
using Nito.AsyncEx;

namespace Lib.Tunnel.Clients
{
    public class NgrokTunnelClient : ITunnelClient
    {
        private HttpClient http;
        private NgrokHttpClient ngrokHttpClient;
        public int[] Ports { get; set; } = new int[] { 10000, 10001 };
        public Dictionary<int, Uri> Tunnels { get; set; } = new Dictionary<int, Uri>();

        public NgrokTunnelClient(HttpClient http)
        {
            this.http = http;
            this.ngrokHttpClient = new NgrokHttpClient(this.http);
        }

        public void Initialize()
        {
            foreach (int port in Ports)
            {
                var tunnel = GetUri(port);
                if (tunnel != null)
                {
                    Tunnels.Add(port, tunnel);
                }
            }
        }

        private async Task<Uri> GetUriAsync(int port)
        {
            var tunnels = await ngrokHttpClient.ListTunnelsAsync();
            var tunnel = tunnels.FirstOrDefault(t => t.Config.Address.EndsWith(port.ToString()));
            if (tunnel != null)
            {
                return new Uri(tunnel.PublicURL);
            }

            return null;
        }

        private Uri GetUri(int port)
        {
            var task = Task.Run(() => AsyncContext.Run(() => GetUriAsync(port)));
            return task.GetAwaiter().GetResult();
        }
    }
}