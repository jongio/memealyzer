using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WebApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");


            var serverHost = string.IsNullOrEmpty(builder.Configuration["API_ENDPOINT"]) ?
                builder.HostEnvironment.BaseAddress :
                builder.Configuration["API_ENDPOINT"];

            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(serverHost) });

            await builder.Build().RunAsync();
        }
    }
}
