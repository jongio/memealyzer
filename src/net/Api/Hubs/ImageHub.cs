using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Lib;
using System.Threading;

namespace Api.Hubs
{
    public class ImageHub : Hub
    {
        private Clients clients;

        public ImageHub(Clients clients)
        {
            this.clients = clients;
        }

        public async Task StartPoll(string id)
        {
            for (int i = 0; i < 1000; i++)
            {
                Image image = await clients.DataProvider.GetImageAsync(id);
                if (image != null)
                {
                    await Clients.All.SendAsync("ReceiveImage", image);
                    break;
                }
                else
                {
                    Thread.Sleep(1000);
                }
            }
        }
    }
}