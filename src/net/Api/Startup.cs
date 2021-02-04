using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using DotNetEnv;
using Microsoft.AspNetCore.ResponseCompression;
using Lib;
using Api.Services;

namespace Api
{
    public class Startup
    {
        readonly string CorsOrigins = "CorsOrigins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Envs.Load();

            services.AddSingleton(typeof(Clients));
            services.AddHostedService<DataHostedService>();
            services.AddSignalR();
            services.AddControllers();
            services.AddCors(options =>
            {
                options.AddPolicy(CorsOrigins,
                    builder => builder //.AllowAnyOrigin() 
                    //.WithOrigins(new string[] { "http://localhost", "https://*.apps.codespaces.githubusercontent.com"})
                    //.SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed(origin => true)
                    .AllowCredentials());
            });

            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.Use(async (context, next) =>
                    {
                        context.Response.OnStarting(() =>
                        {
                            context.Response.Headers["Access-Control-Allow-Origin"] = "https://3778f1c2-941c-4481-b078-087af5b82b01-1080.apps.codespaces.githubusercontent.com";
                            return Task.FromResult(0);
                        });

                        // Call the next delegate/middleware in the pipeline
                        await next();
                    });

            app.UseCors(CorsOrigins);

            app.UseResponseCompression();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
