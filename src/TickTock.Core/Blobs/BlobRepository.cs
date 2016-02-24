using System;

namespace TickTock.Core.Blobs
{
    public class BlobRepository
    {
        public Func<Guid, Blob> GetById { get; set; }

        public Func<byte[], Blob> Add { get; set; }
    }
}