using Nancy;

namespace TickTock.Gate.Modules
{
    public class BlobsModule : NancyModule
    {
        public BlobsModule()
            : base("/blobs")
        {
            Post["/"] = parameters => HttpStatusCode.OK;
            Get["/{id}"] = parameters => HttpStatusCode.OK;
        }
    }
}