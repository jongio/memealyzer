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
            Envs.Load();

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
                    try
                    {
                        Console.WriteLine(message.MessageText);

                        // Deserialize Message
                        var image = clients.DataProvider.DeserializeImage(message.MessageText);

                        // Extract Text from Image
                        try
                        {
                            var recognizeContentOperation = await clients.FormRecognizerClient.StartRecognizeContentFromUriAsync(new Uri(image.BlobUri));
                            var recognizeContentCompletion = await recognizeContentOperation.WaitForCompletionAsync();
                            var content = recognizeContentCompletion.Value;
                            var text = content.SelectMany(page => page.Lines).Aggregate(new StringBuilder(), (a, b) =>
                            {
                                a.Append($"{b.Text} ");
                                return a;
                            });

                            image.Text = text.ToString();
                        }
                        catch (Exception ex)
                        {
                            image.Error = ex.Message.ToString();
                            Console.WriteLine(ex.ToString());
                        }


                        if (!string.IsNullOrEmpty(image.Text))
                        {
                            try
                            {
                                Console.WriteLine($"Image Text: {image.Text}");

                                // Analyize Text Sentiment
                                var documentSentiment = await clients.TextAnalyticsClient.AnalyzeSentimentAsync(image.Text);
                                image.Sentiment = documentSentiment.Value.Sentiment.ToString();

                                Console.WriteLine($"Image Sentiment: {image.Sentiment}");
                            }
                            catch (Exception ex)
                            {
                                image.Error = ex.Message.ToString();
                                Console.WriteLine(ex.ToString());
                            }
                        }
                        else
                        {
                            image.Text = "No Text Extracted from Image.";
                            Console.WriteLine(image.Text);
                        }

                        image.Status = "Completed";
                        
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
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
                await Task.Delay(TimeSpan.FromSeconds(Env.GetInt("AZURE_STORAGE_QUEUE_RECEIVE_SLEEP", 1)));
            }
        }
    }
}
