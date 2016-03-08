using Nancy;
using System;
using System.IO;
using System.Linq;
using TickTock.Core.Blobs;
using TickTock.Core.Extensions;

namespace TickTock.Gate.Modules
{
    public class BlobsModule : NancyModule
    {
        private readonly BlobRepository repository;

        public BlobsModule(BlobRepository repository)
            : base("/api/blobs")
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
                BlobCreation creation = repository.New(content);

                return ToResponse(creation.GetBlob());
            }
        }

        private Response HandleGetBlob(Guid? identifier)
        {
            Blob blob = repository.GetById(identifier.Value);

            if (blob == null)
                return HttpStatusCode.NotFound;

            return ToResponse(blob);
        }

        private Response ToResponse(Blob blob)
        {
            return Response.AsJson(new
            {
                id = blob.Identifier.ToHex(),
                size = blob.GetSize(),
                hash = blob.GetHash(),
                files = GetFiles(blob)
            });
        }

        private static object GetFiles(Blob blob)
        {
            return blob.GetFiles().Select(FromFile).ToArray();
        }

        private static object FromFile(BlobFile file)
        {
            return new
            {
                name = file.Name,
                path = file.Path
            };
        }
    }
}