using System;

namespace TickTock.Core.Executions
{
    public class JobExecutionSchedule
    {
        public DateTime Started { get; set; }

        public DateTime? Completed { get; set; }
    }
}