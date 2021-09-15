using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Azure.Core.Diagnostics;
using Azure.Security.KeyVault.Secrets;
using DotNetEnv;
using Lib;
using Lib.Configuration;
using Lib.Tunnel;

[assembly: FunctionsStartup(typeof(Memealyzer.Startup))]
namespace Memealyzer
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            if (Config.UseAzuriteQueue)
            {
                // Bumping up the MaxPollingInterval so we don't get throttled by our proxy server
                builder.Services.PostConfigure<QueuesOptions>(options => options.MaxPollingInterval = Config.StorageQueueMaxPollingInterval);
            }
        }

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            // We use the following value to indicate which function to enable/disable.  That way our function isn't listening for messages when it doesn't have to.
            var storageQueueEnabled = Config.MessagingType == "STORAGE_QUEUE";

            // These settings will be the same locally and in Azure - we just read them from config and set to appSettings.
            var settings = new Dictionary<string, string> {
                { "MessagingType", Config.MessagingType },
                { "ClientSyncQueueName", Config.ClientSyncQueueName },
                { "AzureWebJobs.StorageQueueFunctionRun.Disabled", (!storageQueueEnabled).ToString() },
                { "AzureWebJobs.ServiceBusFunctionRun.Disabled", storageQueueEnabled.ToString()}
            };

            // These settings only need to be set locally as in prod they are set as app setting values of the host.
            if (Config.IsDevelopment)
            {
                using var listener = AzureEventSourceListener.CreateConsoleLogger();

                settings.Add("AzureSignalRConnectionString:serviceUri", Config.SignalREndpoint.ToString());
                settings.Add("ServiceBusConnection:fullyQualifiedNamespace", Config.ServiceBusNamespace);

                if (Config.UseAzuriteQueue)
                {
                    // If using AzuriteQueue, then we need to use the Azurite connection string that has proxy settings 
                    settings.Add("StorageConnection", Config.AzuriteProxyConnectionString);

                    // NONE OF THE SETTINGS BELOW WORKED. See Configure method above for what worked.
                    // We need to set the maxPollingInterval so we don't get throttled by the proxy
                    // settings.Add("AzureFunctionsJobHost__Extensions__Queues__MaxPollingInterval", Config.StorageQueueMaxPollingInterval.ToString());
                    // settings.Add("AzureFunctionsJobHost.Extensions.Queues.MaxPollingInterval", Config.StorageQueueMaxPollingInterval.ToString());
                    // settings.Add("AzureFunctionsJobHost:Extensions:Queues:MaxPollingInterval", Config.StorageQueueMaxPollingInterval.ToString());
                }
                else
                {
                    // If not using Azurite, then just set to the Azure hosted queue endpoint.
                    settings.Add("StorageConnection:queueServiceUri", Config.StorageQueueEndpoint.ToString());
                }
            }

            builder.ConfigurationBuilder
                .AddInMemoryCollection(settings)
                .AddEnvironmentVariables();
        }
    }
}