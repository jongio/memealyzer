using System;

namespace Lib.Model
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
        string Color { get; }
        string Error { get; set; }
        bool Complete { get; }
    }
}
