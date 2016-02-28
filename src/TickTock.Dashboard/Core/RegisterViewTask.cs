using Nancy.Bootstrapper;
using Nancy.ViewEngines;

namespace TickTock.Dashboard.Core
{
    public class RegisterViewTask : IApplicationStartup
    {
        public void Initialize(IPipelines pipelines)
        {
            ResourceViewLocationProvider
                .RootNamespaces
                .Add(typeof(RegisterViewTask).Assembly, "TickTock.Dashboard.Views");
        }
    }
}