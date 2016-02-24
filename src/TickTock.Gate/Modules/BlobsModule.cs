using Nancy;
using System;
using System.IO;
using TickTock.Core.Blobs;
using TickTock.Core.Extensions;

namespace TickTock.Gate.Modules
{
    public class BlobsModule : NancyModule
    {
        private readonly BlobRepository repository;

        public BlobsModule(BlobRepository repository)
            : base("/blobs")
        {
            this.repository = repository;

            Post["/"] = parameters => HandlePostBlob();
            Get["/{id}"] = parameters => HandleGetBlob(parameters.Id);
        }

        private Response HandlePostBlob()
        {
            using (MemoryStream memory = new MemoryStream())
            {
                Request.Body.CopyTo(memory);
                memory.Seek(0, SeekOrigin.Begin);

                byte[] content = memory.ToArray();
                Blob blob = repository.Add(content);

                return Response.AsJson(new
                {
                    id = blob.Identifier.ToHex()
                });
            }
        }

        private Response HandleGetBlob(Guid? identifier)
        {
            Blob blob = repository.GetById(identifier.Value);

            if (blob == null)
                return HttpStatusCode.NotFound;

            return Response.AsJson(new
            {
                size = blob.GetSize(),
                hash = blob.GetHash()
            });
        }
    }
}