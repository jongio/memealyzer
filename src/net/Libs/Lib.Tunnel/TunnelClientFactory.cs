using System;
using System.Net.Http;
using Lib.Tunnel.Clients;

namespace Lib.Tunnel
{
    public static class TunnelClientFactory
    {
        public static ITunnelClient Get(string type, HttpClient httpClient)
        {
            switch (type)
            {
                case "NGROK":
                    return new NgrokTunnelClient(httpClient);
                default:
                    throw new Exception("TUNNEL_TYPE env var not set.");
            }
        }
    }
}