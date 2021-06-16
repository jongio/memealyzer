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

            // var serviceProvider = builder.Services.BuildServiceProvider();
            // var env = serviceProvider.GetRequiredService<IHostEnvironment>();
            // var appDirectory = serviceProvider.GetRequiredService<IOptions<ExecutionContextOptions>>().Value.AppDirectory;

            // IWebJobsBuilders instance
            // var webJobsBuilder = builder.Services.AddWebJobs(x => { return; });
            // webJobsBuilder.AddAzureStorageQueues(options => options.MaxPollingInterval = TimeSpan.FromMinutes(1));



            // var executioncontextoptions = builder.Services.BuildServiceProvider()
            //                 .GetService<IOptions<ExecutionContextOptions>>().Value;
            // var currentDirectory = executioncontextoptions.AppDirectory;

            // var serviceProvider = builder.Services.BuildServiceProvider();
            // var existingConfig = serviceProvider.GetService<IConfiguration>();

            // var rootPath = currentDirectory;
            // var config = new ConfigurationBuilder()
            //     .AddConfiguration(existingConfig)
            //     .AddInMemoryCollection(settings)
            //     .AddEnvironmentVariables()
            //     .Build();

            // builder.Services.Replace(new ServiceDescriptor(typeof(IConfiguration), config));

            /////////////////


            // var providers = new List<IConfigurationProvider>();
            // foreach (var descriptor in builder.Services.Where(descriptor => descriptor.ServiceType == typeof(IConfiguration)).ToList())

            // {
            //     var existingConfiguration = descriptor.ImplementationInstance as IConfigurationRoot;
            //     if (existingConfiguration is null)
            //     {
            //         continue;
            //     }
            //     providers.AddRange(existingConfiguration.Providers);
            //     builder.Services.Remove(descriptor);
            // }

            // var executioncontextoptions = builder.Services.BuildServiceProvider()
            //     .GetService<IOptions<ExecutionContextOptions>>().Value;
            // var currentDirectory = executioncontextoptions.AppDirectory;

            // var config = new ConfigurationBuilder()
            //     .AddInMemoryCollection(settings)
            //     .AddEnvironmentVariables();

            // providers.AddRange(config.Build().Providers);

            // builder.Services.AddSingleton<IConfiguration>(new ConfigurationRoot(providers));

        }
    }
}