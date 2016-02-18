using Nancy;

namespace TickTock.Gate.Modules
{
    public class JobsModule : NancyModule
    {
        public JobsModule()
            : base("/jobs")
        {
            Get["/"] = parameters => HttpStatusCode.OK;
            Post["/"] = parameters => HttpStatusCode.OK;
            Patch["/{job}"] = parameters => HttpStatusCode.OK;
            Delete["/{job}"] = parameters => HttpStatusCode.OK;

            Get["/{job}/versions"] = parameters => HttpStatusCode.OK;
            Post["/{job}/versions"] = parameters => HttpStatusCode.OK;
            Patch["/{job}/versions/{version}"] = parameters => HttpStatusCode.OK;
            Delete["/{job}/versions/{version}"] = parameters => HttpStatusCode.OK;
        }
    }
}