using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;

namespace Memealyzer.Functions
{
    public static class ServiceBusFunction
    {
        [FunctionName(nameof(RunServiceBusFunction))]
        public static async Task RunServiceBusFunction(
            [ServiceBusTrigger("%ClientSyncQueueName%", Connection = "ServiceBusConnection")] string message,
            [SignalR(HubName = "SERVICE_BUS_QUEUE")] IAsyncCollector<SignalRMessage> messages,
            ILogger log)
        {
            log.LogInformation(message);
            await Messages.SendAsync(messages, message);
        }
    }
}