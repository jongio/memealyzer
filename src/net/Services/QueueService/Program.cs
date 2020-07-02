using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure;
using Azure.AI.FormRecognizer;
using Azure.AI.TextAnalytics;
using Azure.Cosmos;
using Azure.Cosmos.Serialization;
using Azure.Identity;
using Azure.Storage.Queues;
using DotNetEnv;
using Lib;
using Azure.Core.Diagnostics;

namespace QueueService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Env.Load("../../../../.env");

            //using var listener = AzureEventSourceListener.CreateConsoleLogger();

            var cred = Identity.GetCredentialChain();

            // QueueClient
            var queueServiceClient = new QueueServiceClient(
                new Uri(Env.GetString("AZURE_STORAGE_QUEUE_ENDPOINT")), cred);

            var queueClient = queueServiceClient.GetQueueClient(Env.GetString("AZURE_STORAGE_QUEUE_NAME"));
            await queueClient.CreateIfNotExistsAsync();

            // FormRecognizerClient
            var formRecognizerClient = new FormRecognizerClient(
                new Uri(Env.GetString("AZURE_FORM_RECOGNIZER_ENDPOINT")),
                cred);

            // TextAnalyticsClient
            var textAnalyticsClient = new TextAnalyticsClient(
                new Uri(Env.GetString("AZURE_TEXT_ANALYTICS_ENDPOINT")),
                cred);

            // CosmosClient
            var cosmosClient = new CosmosClient(
                Env.GetString("AZURE_COSMOS_ENDPOINT"),
                Env.GetString("AZURE_COSMOS_KEY"),
                new CosmosClientOptions
                {
                    Diagnostics = {
                        IsLoggingEnabled = false
                    },
                    ConnectionMode = ConnectionMode.Direct,
                    ConsistencyLevel = ConsistencyLevel.Session,
                    SerializerOptions = new CosmosSerializationOptions { PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase, IgnoreNullValues = true },
                });

            // Create Database and Container
            var databaseResponse = await cosmosClient.CreateDatabaseIfNotExistsAsync(Env.GetString("AZURE_COSMOS_DB"));
            var containerResponse = await databaseResponse.Database.CreateContainerIfNotExistsAsync(new ContainerProperties(Env.GetString("AZURE_COSMOS_CONTAINER"), "/uid"));

            while (true)
            {
                Console.WriteLine("Receiving Messages...");

                // Get Messages
                var messages = await queueClient.ReceiveMessagesAsync(maxMessages: Env.GetInt("AZURE_STORAGE_QUEUE_MSG_COUNT", 10));

                foreach (var message in messages.Value)
                {
                    Console.WriteLine(message.MessageText);

                    // Deserialize Message
                    var image = JsonSerializer.Deserialize<Image>(message.MessageText,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

                    // Extract Text from Image
                    var recognizeContentOperation = await formRecognizerClient.StartRecognizeContentFromUriAsync(new Uri(image.BlobUri));
                    var recognizeContentCompletion = await recognizeContentOperation.WaitForCompletionAsync();
                    var content = recognizeContentCompletion.Value;
                    var text = content.SelectMany(page => page.Lines).Aggregate(new StringBuilder(), (a, b) =>
                    {
                        a.Append($"{b.Text} ");
                        return a;
                    });

                    image.Text = text.ToString();

                    if (!string.IsNullOrEmpty(image.Text))
                    {
                        Console.WriteLine($"Image Text: {image.Text}");

                        // Analyize Text Sentiment
                        var documentSentiment = await textAnalyticsClient.AnalyzeSentimentAsync(image.Text);
                        image.Sentiment = documentSentiment.Value.Sentiment.ToString();

                        Console.WriteLine($"Image Sentiment: {image.Sentiment}");
                    }
                    else
                    {
                        Console.WriteLine("No Text Extracted from Image.");
                    }

                    // Create Cosmos Document
                    var document = await containerResponse.Container.UpsertItemAsync(image);

                    Console.WriteLine($"Cosmos Document Saved: {document.Value.Id}");

                    // Delete Queue Message
                    var deleteResponse = await queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);

                    Console.WriteLine($"Queue Message Deleted: {message.MessageId}");

                }
                await Task.Delay(TimeSpan.FromSeconds(Env.GetInt("AZURE_STORAGE_QUEUE_RECEIVE_SLEEP", 1)));
            }
        }
    }
}
