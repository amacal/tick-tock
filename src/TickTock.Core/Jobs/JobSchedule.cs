using System;

namespace TickTock.Core.Jobs
{
    public class JobSchedule
    {
        public Func<DateTime?, DateTime?> Next { get; set; }
    }
}