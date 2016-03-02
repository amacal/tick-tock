using Nancy;
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
            Get["/application.css"] = parameters => GetDashboardStyles();
        }

        private Response GetDashboardView()
        {
            Func<Stream> stream = () =>
            {
                Assembly assembly = typeof(DashboardModule).Assembly;
                string resource = "TickTock.Gate.Application.Dashboard.xhtml";

                return assembly.GetManifestResourceStream(resource);
            };

            return Response.FromStream(stream, "text/html");
        }

        private Response GetDashboardScript()
        {
            Func<Stream> stream = () =>
            {
                Assembly assembly = typeof(DashboardModule).Assembly;
                string resource = "TickTock.Gate.Application.Dashboard.js";

                return assembly.GetManifestResourceStream(resource);
            };

            return Response.FromStream(stream, "text/javascript");
        }

        private Response GetDashboardStyles()
        {
            Func<Stream> stream = () =>
            {
                Assembly assembly = typeof(DashboardModule).Assembly;
                string resource = "TickTock.Gate.Application.Dashboard.css";

                return assembly.GetManifestResourceStream(resource);
            };

            return Response.FromStream(stream, "text/css");
        }
    }
}