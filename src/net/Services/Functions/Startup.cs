using System;
using System.Collections.Generic;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Azure.Security.KeyVault.Secrets;
using DotNetEnv;
using Azure.Core.Diagnostics;
using Lib;
using Microsoft.Azure.WebJobs;

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

            //Environment.SetEnvironmentVariable("Values:AzureWebJobsStorage", storageConnectionString.Value.Value);
            var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string> {
                { "ClientSyncQueueConnectionString", storageConnectionString.Value.Value },
                { "AzureSignalRConnectionString", signalRConnectionString.Value.Value }
            })
            .AddEnvironmentVariables().Build();

            builder.Services.AddSingleton<IConfiguration>(config);
        }
    }
}