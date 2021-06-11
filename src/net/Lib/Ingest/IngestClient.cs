using System.Threading.Tasks;
using Lib.Media;
using Lib.Messaging;
using Lib.Model;
using Lib.Storage;

namespace Lib.Ingest
{
    public class IngestClient : IIngestClient
    {
        private IImageClient imageClient;
        private IStorageClient storageClient;
        private IMessagingProvider messagingProvider;

        public IngestClient(IImageClient imageClient, IStorageClient storageClient, IMessagingProvider messagingProvider)
        {
            this.imageClient = imageClient;
            this.storageClient = storageClient;
            this.messagingProvider = messagingProvider;
        }

        public async Task<Image> Ingest(Image image)
        {
            // Get Image Stream
            using (var stream = await imageClient.GetImageStream(image))
            {
                // Upload to Blob
                var blobInfo = await storageClient.UploadBlobAsync(image, stream);
                //Console.WriteLine($"Uploaded to Blob Storage: {blobInfo.Uri}");
            }

            // Send Queue Message
            var sendReceipt = await messagingProvider.ImageQueueClient.SendMessageAsync(new ImageQueueMessage { Image = image });

            //Console.WriteLine($"Added to Queue: {sendReceipt.Message.Id}");
            return image;
        }
    }
}