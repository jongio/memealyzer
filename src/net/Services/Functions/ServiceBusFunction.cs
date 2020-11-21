using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;

namespace Memealyzer.Functions
{
    public static class ServiceBusFunction
    {
        [FunctionName("ServiceBugTrigger")]
        public static async Task Run(
            [ServiceBusTrigger("%AZURE_CLIENT_SYNC_QUEUE_NAME%", Connection = "ServiceBusConnectionString")] string message,
            [SignalR(HubName = "SERVICE_BUS_QUEUE")] IAsyncCollector<SignalRMessage> messages,
            ILogger log)
        {
            await Messages.SendAsync(messages, message);
        }
    }
}