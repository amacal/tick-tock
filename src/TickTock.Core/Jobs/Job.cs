using System;

namespace TickTock.Core.Jobs
{
    public class Job
    {
        public JobHeader Header { get; set; }

        public JobSchedule Schedule { get; set; }

        public Action<Action<JobData>> Extract { get; set; }
    }
}