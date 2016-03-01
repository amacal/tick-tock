using Nancy;
using Nancy.Responses.Negotiation;
using System;
using System.IO;
using System.Reflection;

namespace TickTock.Gate.Modules
{
    public class DashboardModule : NancyModule
    {
        public DashboardModule()
            : base("/dashboard")
        {
            Get["/"] = parameters => GetDashboardView();
            Get["/application.js"] = parameters => GetDashboardScript();
        }

        private Negotiator GetDashboardView()
        {
            return View["Dashboard"];
        }

        private Response GetDashboardScript()
        {
            Func<Stream> stream = () =>
            {
                Assembly assembly = typeof(DashboardModule).Assembly;
                string resource = "TickTock.Gate.Scripts.Dashboard.js";

                return assembly.GetManifestResourceStream(resource);
            };

            return Response.FromStream(stream, "application/javascript");
        }
    }
}