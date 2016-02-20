using Nancy;
using Nancy.ModelBinding;
using System;
using TickTock.Core.Extensions;
using TickTock.Core.Jobs;

namespace TickTock.Gate.Modules
{
    public class JobsModule : NancyModule
    {
        private readonly IJobRepository repository;

        public JobsModule(IJobRepository repository)
            : base("/jobs")
        {
            this.repository = repository;

            Get["/"] = parameters => HttpStatusCode.OK;
            Post["/"] = parameters => HandlePostJob(this.Bind<DynamicDictionary>());
            Get["/{job}"] = parameters => HandleGetJob(parameters.job);
            Patch["/{job}"] = parameters => HttpStatusCode.OK;
            Delete["/{job}"] = parameters => HttpStatusCode.OK;

            Get["/{job}/versions"] = parameters => HttpStatusCode.OK;
            Delete["/{job}/versions/{version}"] = parameters => HttpStatusCode.OK;
        }

        public Response HandlePostJob(dynamic model)
        {
            JobData data = new JobData
            {
                Name = model.name,
                Description = model.description,
                Blob = model.blob
            };

            JobHeader header = repository.Add(data);
            Guid identifier = header.Identifier;

            return Response.AsJson(new
            {
                id = identifier.ToHex()
            });
        }

        public Response HandleGetJob(Guid? identifier)
        {
            Job job = repository.GetById(identifier.Value);

            if (job == null)
                return HttpStatusCode.NotFound;

            return Response.AsJson(new
            {
                id = job.Header.Identifier.ToHex(),
                version = job.Header.Version,
                name = job.Data.Name,
                description = job.Data.Description,
                blob = job.Data.Blob.ToHex()
            });
        }
    }
}