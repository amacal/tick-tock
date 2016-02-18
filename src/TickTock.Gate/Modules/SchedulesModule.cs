using Nancy;

namespace TickTock.Gate.Modules
{
    public class SchedulesModule : NancyModule
    {
        public SchedulesModule()
            : base("/schedules")
        {
            Get["/"] = parameters => HttpStatusCode.OK;
            Get["/{schedule}"] = parameters => HttpStatusCode.OK;
            Get["/{schedule}/logs"] = parameters => HttpStatusCode.OK;
        }
    }
}