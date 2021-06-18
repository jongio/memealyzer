using System;
using System.Collections.Generic;

namespace Lib.Tunnel
{
    public interface ITunnelClient
    {
        void Initialize();
        int[] Ports { get; set; }
        Dictionary<int, Uri> Tunnels { get; set; }
    }
}