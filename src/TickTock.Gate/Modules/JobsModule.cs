using Nancy;
using Nancy.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using TickTock.Core.Executions;
using TickTock.Core.Extensions;
using TickTock.Core.Jobs;

namespace TickTock.Gate.Modules
{
    public class JobsModule : NancyModule
    {
        private readonly JobRepository jobs;
        private readonly JobExecutionRepository executions;

        public JobsModule(JobRepository jobs, JobExecutionRepository executions)
            : base("/api/jobs")
        {
            this.jobs = jobs;
            this.executions = executions;

            Get["/"] = parameters => HandleGetAllJobs();
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

        private Response HandleGetAllJobs()
        {
            Job[] all = jobs.All(with => { });
            List<object> model = new List<object>(all.Length);

            foreach (Job job in all)
            {
                JobExecution execution = executions.GetByJob(job.Header).FirstOrDefault();
                DateTime? nextRun = execution != null ? execution.NextRun(job.Schedule) : job.Schedule.Next(null);

                job.Extract(data =>
                {
                    model.Add(new
                    {
                        id = job.Header.Identifier.ToHex(),
                        version = job.Header.Version,
                        name = data.Name,
                        description = data.Description,
                        executable = data.Executable,
                        arguments = data.Arguments,
                        blob = data.Blob.ToHex(),
                        nextRun = nextRun
                    });
                });
            }

            return Response.AsJson(model);
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

            JobHeader header = jobs.New(data);
            Guid identifier = header.Identifier;

            return Response.AsJson(new
            {
                id = identifier.ToHex()
            });
        }

        public Response HandleGetJob(Guid? identifier)
        {
            Response response = HttpStatusCode.NotFound;
            Job job = jobs.Single(with =>
            {
                with.Identifier = identifier.Value;
            });

            if (job != null)
            {
                job.Extract(data =>
                {
                    response = Response.AsJson(new
                    {
                        id = job.Header.Identifier.ToHex(),
                        version = job.Header.Version,
                        name = data.Name,
                        description = data.Description,
                        executable = data.Executable,
                        arguments = data.Arguments,
                        blob = data.Blob.ToHex()
                    });
                });
            }

            return response;
        }
    }
}