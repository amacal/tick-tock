using System;

namespace TickTock.Core.Blobs
{
    public class Blob
    {
        public Guid Identifier { get; set; }

        public string Hash { get; set; }

        public long Size { get; set; }
    }
}