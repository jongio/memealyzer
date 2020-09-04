using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Lib;

namespace Memealyzer.Functions
{
    public static class ClientSyncFunction
    {
        [FunctionName("negotiate")]
        public static SignalRConnectionInfo Negotiate(
            [HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req,
            [SignalRConnectionInfo(HubName = "imagehub")]
            SignalRConnectionInfo connectionInfo)
        {
            return connectionInfo;
        }

        [FunctionName("QueueTrigger")]
        public static async Task Run(
            [QueueTrigger(
                "%AZURE_STORAGE_CLIENT_SYNC_QUEUE_NAME%",
                Connection = "ClientSyncQueueConnectionString")]
                string queueMessage,
            [SignalR(
                HubName = "imagehub")]
                IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            var image = JsonSerializer.Deserialize<Image>(queueMessage,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            await signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "ReceiveImage",
                    Arguments = new[] { image }
                });
        }
    }
}