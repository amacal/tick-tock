using System;

namespace TickTock.Core.Blobs
{
    public class BlobCreation
    {
        public bool Success { get; set; }

        public Func<Blob> GetBlob { get; set; }
    }
}