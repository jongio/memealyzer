using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Azure.Core;
using Azure.Identity;
using DotNetEnv;
using System;

namespace azsdkdemoapi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            Env.Load("../../.env");

            services.AddControllers();

            // 1. Add Tracing policies via AddSingleton
            services.AddSingleton<SimpleTracingPolicy>();

            // 2. Add Azure SDK clients via AddAzureClients
            services.AddAzureClients(builder =>
            {
                // 3. Read in default settings, including retry policies
                builder.ConfigureDefaults(Configuration.GetSection("AzureDefaults"));

                // 4. Add the blob client
                builder.AddBlobServiceClient(new Uri(Environment.GetEnvironmentVariable("AZURE_STORAGE_BLOB_URI")))
                    .ConfigureOptions((options, provider) =>
                    {
                        options.AddPolicy(provider.GetService<SimpleTracingPolicy>(), HttpPipelinePosition.PerCall);
                    });

                // 5. Add the queue client
                builder.AddQueueServiceClient(new Uri(Environment.GetEnvironmentVariable("AZURE_STORAGE_QUEUE_URI")));

                // 6. Add DefaultAzureCredential for all clients
                builder.UseCredential(new DefaultAzureCredential());
            });

            // 7. Add App Insights
            services.AddApplicationInsightsTelemetry();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
