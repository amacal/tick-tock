using Nancy;
using Nancy.ModelBinding;
using System;
using TickTock.Core.Extensions;
using TickTock.Core.Jobs;

namespace TickTock.Gate.Modules
{
    public class JobsModule : NancyModule
    {
        private readonly JobRepository repository;

        public JobsModule(JobRepository repository)
            : base("/jobs")
        {
            this.repository = repository;

            Get["/"] = parameters => HttpStatusCode.OK;
            Post["/"] = parameters => HandlePostJob(this.Bind<DynamicDictionary>());
            Get["/{job}"] = parameters => HandleGetJob(parameters.job);
            Patch["/{job}"] = parameters => HttpStatusCode.OK;
            Delete["/{job}"] = parameters => HttpStatusCode.OK;

            Get["/{job}/versions"] = parameters => HttpStatusCode.OK;
            Get["/{job}/versions/{version}"] = parameters => HttpStatusCode.OK;
            Delete["/{job}/versions/{version}"] = parameters => HttpStatusCode.OK;

            Get["/{job}/schedule"] = parameters => HttpStatusCode.OK;
            Patch["/{job}/schedule"] = parameters => HttpStatusCode.OK;

            Get["/{job}/executions"] = parameters => HttpStatusCode.OK;
            Get["/{job}/executions/newest"] = parameters => HttpStatusCode.OK;
            Get["/{job}/executions/{execution}"] = parameters => HttpStatusCode.OK;
        }

        public Response HandlePostJob(dynamic model)
        {
            JobData data = new JobData
            {
                Name = model.name,
                Description = model.description,
                Executable = model.executable,
                Arguments = model.arguments,
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
                executable = job.Data.Executable,
                arguments = job.Data.Arguments,
                blob = job.Data.Blob.ToHex()
            });
        }
    }
}