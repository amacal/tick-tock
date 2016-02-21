using System;

namespace TickTock.Core.Jobs
{
    public class JobSchedule
    {
        public bool IsEnabled { get; set; }

        public TimeSpan Interval { get; set; }
    }
}