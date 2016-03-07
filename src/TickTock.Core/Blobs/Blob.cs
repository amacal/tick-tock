using System;

namespace TickTock.Core.Blobs
{
    public class Blob
    {
        public Guid Identifier { get; set; }

        public Func<long> GetSize { get; set; }

        public Func<string> GetHash { get; set; }

        public Func<BlobFileCollection> GetFiles { get; set; }

        public Func<string, BlobDeployment> DeployTo { get; set; }
    }
}