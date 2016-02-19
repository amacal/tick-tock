using Nancy;
using System;
using System.IO;
using TickTock.Core.Blobs;

namespace TickTock.Gate.Modules
{
    public class BlobsModule : NancyModule
    {
        private readonly IBlobRepository repository;

        public BlobsModule(IBlobRepository repository)
            : base("/blobs")
        {
            this.repository = repository;

            Post["/"] = parameters => HandlePostBlob();
            Get["/{id}"] = parameters => HttpStatusCode.OK;
        }

        private Response HandlePostBlob()
        {
            using (MemoryStream memory = new MemoryStream())
            {
                Request.Body.CopyTo(memory);
                memory.Seek(0, SeekOrigin.Begin);

                Guid identifier = repository.Add(memory.ToArray());
                var response = new { id = identifier };

                return Response.AsJson(response);
            }
        }
    }
}