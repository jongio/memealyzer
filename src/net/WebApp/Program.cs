using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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

            builder.Services.AddHttpClient<ApiServiceClient>(client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
            builder.Services.AddHttpClient<FunctionsServiceClient>(client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
            await builder.Build().RunAsync();
        }
    }
}
