using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DotNetEnv;
using Lib;

namespace QueueService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Env.Load("../../../../.env");

            //using var listener = AzureEventSourceListener.CreateConsoleLogger();

            var clients = new Clients();
            await clients.InitializeAsync();

            while (true)
            {
                Console.WriteLine("Receiving Messages...");

                // Get Messages
                var messages = await clients.QueueClient.ReceiveMessagesAsync(maxMessages: Env.GetInt("AZURE_STORAGE_QUEUE_MSG_COUNT", 10));

                foreach (var message in messages.Value)
                {
                    Console.WriteLine(message.MessageText);


                    // Deserialize Message
                    var image = clients.DataProvider.DeserializeImage(message.MessageText);

                    // Extract Text from Image
                    var recognizeContentOperation = await clients.FormRecognizerClient.StartRecognizeContentFromUriAsync(new Uri(image.BlobUri));
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
                        var documentSentiment = await clients.TextAnalyticsClient.AnalyzeSentimentAsync(image.Text);
                        image.Sentiment = documentSentiment.Value.Sentiment.ToString();

                        Console.WriteLine($"Image Sentiment: {image.Sentiment}");
                    }
                    else
                    {
                        Console.WriteLine("No Text Extracted from Image.");
                    }

                    // Save Document
                    image = await clients.DataProvider.UpsertImageAsync(image);

                    Console.WriteLine($"Document Saved: {image.Id}");

                    // Delete Queue Message
                    var deleteResponse = await clients.QueueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);

                    Console.WriteLine($"Queue Message Deleted: {message.MessageId}");

                    // Enqueue message to Client Sync Queue
                    var sendReceipt = await clients.ClientSyncQueueClient.SendMessageAsync(
                        Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize<IImage>(image,
                            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })
                        )));

                    Console.WriteLine($"Added to Client Sync Queue: {sendReceipt.Value.MessageId}");
                }
                await Task.Delay(TimeSpan.FromSeconds(Env.GetInt("AZURE_STORAGE_QUEUE_RECEIVE_SLEEP", 1)));
            }
        }
    }
}
