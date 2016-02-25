using Nancy;
using Nancy.TinyIoc;
using System.IO;
using TickTock.Core.Blobs;
using TickTock.Core.Executions;
using TickTock.Core.Jobs;

namespace TickTock.Service
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        private readonly string location;

        public Bootstrapper(string location)
        {
            this.location = location;
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            container.Register(BlobRepositoryFactory.Create(Path.Combine(location, "blobs")));
            container.Register(JobExecutionRepositoryFactory.Create(Path.Combine(location, "executions")));
            container.Register(JobRepositoryFactory.Create(Path.Combine(location, "jobs")));
        }
    }
}