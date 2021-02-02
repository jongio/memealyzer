using System;
using System.Collections.Generic;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Azure.Core.Diagnostics;
using Azure.Security.KeyVault.Secrets;
using DotNetEnv;
using Lib;


[assembly: FunctionsStartup(typeof(Memealyzer.Startup))]
namespace Memealyzer
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            //using var listener = AzureEventSourceListener.CreateConsoleLogger();

            Envs.Load();

            // KeyVault
            var secretClient = new SecretClient(Config.KeyVaultEndpoint, Identity.GetCredentialChain());
            var storageConnectionString = secretClient.GetSecret(Config.StorageConnectionStringSecretName);
            var signalRConnectionString = secretClient.GetSecret(Config.SignalRConnectionStringSecretName);
            var serviceBusCnnectionString = secretClient.GetSecret(Config.ServiceBusConnectionStringSecretName);

            
            //Environment.SetEnvironmentVariable("Values:AzureWebJobsStorage", storageConnectionString.Value.Value);
            var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string> {
                { "StorageConnectionString", storageConnectionString.Value.Value },
                { "AzureSignalRConnectionString", signalRConnectionString.Value.Value },
                { "ServiceBusConnectionString", serviceBusCnnectionString.Value.Value },
                { "MessagingType", Config.MessagingType },
                { "ClientSyncQueueName", Config.ClientSyncQueueName}
            })
            .AddEnvironmentVariables().Build();

            builder.Services.AddSingleton<IConfiguration>(config);
        }
    }
}