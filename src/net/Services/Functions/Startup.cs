using System;
using System.Collections.Generic;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Azure.Security.KeyVault.Secrets;
using DotNetEnv;
using Lib;
using Microsoft.Azure.WebJobs;

[assembly: FunctionsStartup(typeof(Memealyzer.Startup))]
namespace Memealyzer
{

    public class Startup : FunctionsStartup
    {
        // public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        // {
        //     FunctionsHostBuilderContext context = builder.GetContext();

        //     Envs.Load();

        //     // KeyVault
        //     var secretClient = new SecretClient(new Uri(Env.GetString("AZURE_KEYVAULT_ENDPOINT")), Identity.GetCredentialChain());
        //     var storageConnectionString = secretClient.GetSecret(Env.GetString("AZURE_STORAGE_CONNECTION_STRING_SECRET_NAME"));
        //     var signalRConnectionString = secretClient.GetSecret(Env.GetString("AZURE_SIGNALR_CONNECTION_STRING_SECRET_NAME"));

        //     //Environment.SetEnvironmentVariable("Values:AzureWebJobsStorage", storageConnectionString.Value.Value);

        //     builder.ConfigurationBuilder
        //     .AddInMemoryCollection(new Dictionary<string, string> {
        //         { "ClientSyncQueueConnectionString", storageConnectionString.Value.Value },
        //         { "AzureSignalRConnectionString", signalRConnectionString.Value.Value }
        //     })
        //     .AddEnvironmentVariables();
        // }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            //FunctionsHostBuilderContext context = builder.GetContext();

            Envs.Load();

            // KeyVault
            var secretClient = new SecretClient(new Uri(Env.GetString("AZURE_KEYVAULT_ENDPOINT")), Identity.GetCredentialChain());
            var storageConnectionString = secretClient.GetSecret(Env.GetString("AZURE_STORAGE_CONNECTION_STRING_SECRET_NAME"));
            var signalRConnectionString = secretClient.GetSecret(Env.GetString("AZURE_SIGNALR_CONNECTION_STRING_SECRET_NAME"));

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