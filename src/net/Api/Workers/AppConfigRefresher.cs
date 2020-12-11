using System;
using System.Threading;
using System.Threading.Tasks;
using Lib;
using Microsoft.Extensions.Hosting;

namespace Api.Workers
{
    public class AppConfigRefresher : BackgroundService
    {
        public AppConfigRefresher(Clients clients)
        {
            Clients = clients;
        }

        public Clients Clients { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                await Clients.RefreshAppConfiguration();
                
                await Task.Delay(TimeSpan.FromSeconds(30));
            }
        }
    }
}