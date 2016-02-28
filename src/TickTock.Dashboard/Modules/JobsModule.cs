using Nancy;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TickTock.Core.Blobs;
using TickTock.Core.Extensions;
using TickTock.Core.Jobs;
using TickTock.Dashboard.Models.Jobs;

namespace TickTock.Dashboard.Modules
{
    public class JobsModule : NancyModule
    {
        public JobsModule(BlobRepository blobRepository, JobRepository jobRepository)
            : base("/dashboard/jobs")
        {
            Get["/"] = parameters =>
            {
                Job[] jobs = jobRepository.GetAll();
                IndexModel model = new IndexModel
                {
                    Jobs = new List<IndexModel.JobModel>()
                };

                foreach (Job job in jobs)
                {
                    model.Jobs.Add(new IndexModel.JobModel
                    {
                        Id = job.Header.Identifier.ToHex(),
                        Version = job.Header.Version.ToString(),
                        Name = job.Data.Name
                    });
                }

                return View["Index", model];
            };

            Get["/{id}"] = parameters =>
            {
                return View["View", new ViewModel { }];
            };

            Get["/new"] = parameters =>
            {
                return View["New", new NewModel { }];
            };

            Post["/new"] = parameters =>
            {
                Blob blob;
                HttpFile file = Request.Files.First();

                using (MemoryStream memory = new MemoryStream())
                {
                    file.Value.CopyTo(memory);
                    blob = blobRepository.Add(memory.ToArray());
                }

                JobData data = new JobData
                {
                    Name = Request.Form.name,
                    Description = Request.Form.description,
                    Executable = Request.Form.executable,
                    Arguments = Request.Form.arguments,
                    Blob = blob.Identifier
                };

                JobHeader header = jobRepository.Add(data);
                string identifier = header.Identifier.ToHex();

                return Response.AsRedirect($"/dashboard/jobs/{identifier}");
            };
        }
    }
}