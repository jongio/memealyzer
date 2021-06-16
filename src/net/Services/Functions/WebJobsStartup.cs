using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Hosting;

[assembly: WebJobsStartup(typeof(Memealyzer.WebJobsStartup))]
namespace Memealyzer
{
    public class WebJobsStartup : IWebJobsStartup
    {
        void IWebJobsStartup.Configure(IWebJobsBuilder builder)
        {
            builder.AddAzureStorageQueues(options => options.MaxPollingInterval = TimeSpan.FromSeconds(7));
        }
    }
}