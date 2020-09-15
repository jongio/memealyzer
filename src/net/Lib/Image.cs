using System;
using System.IO;
using System.Text.Json.Serialization;

namespace Lib
{
    public class Image : IImage
    {
        private string extension;
        private string blobName;
        private string color = "light";

        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonPropertyName("uid")]
        public string Uid { get { return Id; } set { Id = value; } }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("extension")]
        public string Extension
        {
            get
            {
                if (string.IsNullOrEmpty(extension))
                {
                    extension = Path.GetExtension(this.Url);
                }
                return extension;
            }
            set => extension = value;
        }

        [JsonPropertyName("blobName")]
        public string BlobName
        {
            get
            {
                if (string.IsNullOrEmpty(blobName))
                {
                    blobName = $"{Id}{Extension}";
                }
                return blobName;
            }
            set => blobName = value;
        }

        [JsonPropertyName("blobUri")]
        public string BlobUri { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("sentiment")]
        public string Sentiment { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; }

        [JsonPropertyName("createdDate")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("type")]
        public ImageType Type { get; set; } = ImageType.New;

        [JsonPropertyName("color")]
        public string Color
        {
            get
            {
                switch (Sentiment?.ToLower())
                {
                    case "positive":
                        color = "success";
                        break;
                    case "negative":
                        color = "danger";
                        break;
                    case "neutral":
                        color = "warning";
                        break;
                    case "mixed":
                        color = "warning";
                        break;
                    case "loading":
                        color = "white";
                        break;
                }
                return color;
            }
            set
            {
                color = value;
            }
        }
    }
}
