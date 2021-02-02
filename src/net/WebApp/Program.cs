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

            var apiEndpoint = string.IsNullOrEmpty(builder.Configuration["API_ENDPOINT"]) ?
                builder.HostEnvironment.BaseAddress :
                builder.Configuration["API_ENDPOINT"];


            var functionsEndpoint = string.IsNullOrEmpty(builder.Configuration["FUNCTIONS_ENDPOINT"]) ?
                apiEndpoint :
                builder.Configuration["FUNCTIONS_ENDPOINT"];

            builder.Services.AddHttpClient<ApiServiceClient>(client => client.BaseAddress = new Uri(apiEndpoint));

            builder.Services.AddHttpClient<FunctionsServiceClient>(client => client.BaseAddress = new Uri(functionsEndpoint));

            await builder.Build().RunAsync();
        }
    }
}
