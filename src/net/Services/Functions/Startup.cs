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
using Lib.Proxy;

[assembly: FunctionsStartup(typeof(Memealyzer.Startup))]
namespace Memealyzer
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            using var listener = AzureEventSourceListener.CreateConsoleLogger();

            Envs.Load();

            // KeyVault
            var secretClient = new SecretClient(Config.KeyVaultEndpoint, Identity.GetCredentialChain());
            var signalRConnectionString = secretClient.GetSecret(Config.SignalRConnectionStringSecretName);

            // We use the following value to indicate which function to enable/disable.  That way our function isn't listening for messages when it doesn't have to.
            var storageQueueEnabled = Config.MessagingType == "STORAGE_QUEUE";

            var settings = new Dictionary<string, string> {
                { "AzureSignalRConnectionString", signalRConnectionString.Value.Value },
                { "ServiceBusConnection:fullyQualifiedNamespace", Config.ServiceBusNamespace },
                { "MessagingType", Config.MessagingType },
                { "ClientSyncQueueName", Config.ClientSyncQueueName },
                { "AzureWebJobs.StorageQueueFunctionRun.Disabled", (!storageQueueEnabled).ToString() },
                { "AzureWebJobs.ServiceBusFunctionRun.Disabled", storageQueueEnabled.ToString()}
            };

            // If we are using Azurite, then we must set StorageConnection to the Proxy endpoint so the function on Azure can access it
            if (Config.UseAzuriteQueue)
            {
                settings.Add("StorageConnection", Config.AzuriteProxyConnectionString);

                // Bumping up the MaxPollingInterval so we don't trottle our proxy server
                builder.Services.PostConfigure<QueuesOptions>(options => options.MaxPollingInterval = TimeSpan.FromSeconds(4));
            }
            else
            {
                settings.Add("AzureWebJobsStorage:accountName", Config.StorageAccountName);
                settings.Add("StorageConnection:queueServiceUri", Config.StorageQueueEndpoint.ToString());
            }

            var config = new ConfigurationBuilder()
                        .AddInMemoryCollection(settings)
                        .AddEnvironmentVariables()
                        .Build();

            builder.Services.AddSingleton<IConfiguration>(config);
        }
    }
}