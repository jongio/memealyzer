using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Azure.Core;
using Azure.Identity;

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
            services.AddControllers();

            // 1. Add Tracing policies via AddSingleton
            services.AddSingleton<SimpleTracingPolicy>();

            // 2. Add Azure SDK clients via AddAzureClients
            services.AddAzureClients(builder =>
            {
                builder.AddBlobServiceClient(new Uri(Environment.GetEnvironmentVariable("AZURE_STORAGE_BLOB_URI")))
                    .ConfigureOptions((options, provider) =>
                    {
                        options.Retry.MaxRetries = 10;
                        options.Retry.Delay = TimeSpan.FromSeconds(3);
                        options.Diagnostics.IsLoggingEnabled = true;
                        options.AddPolicy(provider.GetService<SimpleTracingPolicy>(), HttpPipelinePosition.PerCall);
                    }).WithCredential(new DefaultAzureCredential());
            });

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
