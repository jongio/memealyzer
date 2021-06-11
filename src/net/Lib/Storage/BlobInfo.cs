using System;
using Azure.Storage.Blobs.Models;

namespace Lib.Storage
{
    public class BlobInfo
    {
        public BlobContentInfo BlobContentInfo { get; set; }
        public Uri Uri { get; set; }
    }
}