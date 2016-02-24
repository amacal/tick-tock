using System;

namespace TickTock.Core.Executions
{
    public class JobExecutionProgress
    {
        public Func<JobExecutionStatus> GetStatus { get; set; }

        public Func<int> GetPid { get; set; }

        public Action OnScheduled { get; set; }

        public Action<int> OnStarted { get; set; }

        public Action OnCompleted { get; set; }
    }
}