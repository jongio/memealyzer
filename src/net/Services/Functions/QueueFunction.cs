using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;

namespace Memealyzer.Functions
{
    public static class QueueFunction
    {
        [FunctionName("QueueTrigger")]
        public static async Task Run(
            [QueueTrigger("%ClientSyncQueueName%", Connection = "StorageConnectionString")] string message,
            [SignalR(HubName = "STORAGE_QUEUE")] IAsyncCollector<SignalRMessage> messages,
            ILogger log)
        {
            await Messages.SendAsync(messages, message);
        }
    }
}