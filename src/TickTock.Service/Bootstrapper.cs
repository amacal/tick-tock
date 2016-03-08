using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using Nancy.ViewEngines;
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

            container.Register(BlobRepositoryFactory.Create(with =>
            {
                with.Location = Path.Combine(location, "blobs");
            }));

            container.Register(JobExecutionRepositoryFactory.Create(with =>
            {
                with.Location = Path.Combine(location, "executions");
            }));

            container.Register(JobRepositoryFactory.Create(with =>
            {
                with.Location = Path.Combine(location, "jobs");
            }));
        }

        protected override NancyInternalConfiguration InternalConfiguration
        {
            get
            {
                return NancyInternalConfiguration.WithOverrides(with =>
                {
                    with.ViewLocationProvider = typeof(ResourceViewLocationProvider);
                });
            }
        }
    }
}