using System;
using Azure.Storage.Blobs.Models;
using Lib.Model;

namespace Lib.Storage
{
    public class BlobInfo
    {
        public BlobContentInfo BlobContentInfo { get; set; }
        public Image Image { get; set; }
    }
}