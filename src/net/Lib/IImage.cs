using System;

namespace Lib
{
    public interface IImage
    {
        string Id { get; set; }
        string Uid { get; set; }
        string Title { get; set; }
        string Url { get; set; }
        string Extension { get; }
        string BlobName { get; }
        string BlobUri { get; set; }
        string Text { get; set; }
        string Sentiment { get; set; }
        string Status { get; set; }
        DateTime CreatedDate { get; set; }
        string Style { get; }
    }
}
