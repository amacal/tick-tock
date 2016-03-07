namespace TickTock.Core.Blobs
{
    public class BlobDeployment
    {
        public string Path { get; set; }

        public BlobFileCollection Files { get; set; }
    }
}