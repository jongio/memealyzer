using System;
using System.Threading;
using System.Threading.Tasks;
using Lib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Api.Services
{
    public class DataHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        public DataHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var clients = scope.ServiceProvider.GetRequiredService<Clients>();
                await clients.InitializeAsync();
            }
        }

        // noop
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}