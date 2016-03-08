using System;

namespace TickTock.Core.Blobs
{
    public class BlobRepository
    {
        public Func<Guid, Blob> GetById { get; set; }

        public Func<byte[], BlobCreation> New { get; set; }
    }
}