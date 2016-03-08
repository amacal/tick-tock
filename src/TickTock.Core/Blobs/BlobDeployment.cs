using System;

namespace TickTock.Core.Blobs
{
    public class BlobDeployment
    {
        public string Path { get; set; }

        public BlobFileCollection Files { get; set; }

        public Func<string, string> Find { get; set; }
    }
}