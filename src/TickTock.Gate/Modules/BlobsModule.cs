using Nancy;
using System;
using System.IO;
using TickTock.Core.Blobs;
using TickTock.Core.Extensions;

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
            Get["/{id}"] = parameters => HandleGetBlob(parameters.Id);
            Get["/{id}/data"] = parameters => HandleGetBlobData(parameters.Id);
        }

        private Response HandlePostBlob()
        {
            using (MemoryStream memory = new MemoryStream())
            {
                Request.Body.CopyTo(memory);
                memory.Seek(0, SeekOrigin.Begin);

                byte[] content = memory.ToArray();
                Guid identifier = repository.Add(content);

                return Response.AsJson(new
                {
                    id = identifier.ToHex()
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
                size = blob.Size,
                hash = blob.Hash
            });
        }

        private Response HandleGetBlobData(Guid? identifier)
        {
            Blob blob = repository.GetById(identifier.Value);

            if (blob == null)
                return HttpStatusCode.NotFound;

            byte[] data = repository.GetData(identifier.Value);
            MemoryStream memory = new MemoryStream(data);

            memory.Seek(0, SeekOrigin.Begin);
            return Response.FromStream(memory, "application/octet-stream");
        }
    }
}