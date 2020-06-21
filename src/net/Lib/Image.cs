using System;
using System.IO;
using System.Text.Json.Serialization;

namespace Lib
{
    public class Image
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [JsonPropertyName("uid")]
        public string Uid { get { return Id; } set { Id = value; } }
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("url")]
        public string Url { get; set; }
        [JsonPropertyName("extension")]
        public string Extension { get { return Path.GetExtension(this.Url); } }
        [JsonPropertyName("blobName")]
        public string BlobName { get { return $"{Id}{Extension}"; } }
        [JsonPropertyName("blobUri")]
        public string BlobUri { get; set; }
        [JsonPropertyName("text")]
        public string Text { get; set; }
        [JsonPropertyName("sentiment")]
        public string Sentiment { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("createdDate")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public ImageType Type = ImageType.New;

        public string Style
        {
            get
            {
                string style = "light";
                switch (Sentiment)
                {
                    case "Positive":
                        style = "success";
                        break;
                    case "Negative":
                        style = "danger";
                        break;
                    case "Neutral":
                        style = "dark";
                        break;
                    case "Mixed":
                        style = "warning";
                        break;
                    case "Loading":
                        style = "white";
                        break;
                }
                return style;
            }
        }
    }
}
