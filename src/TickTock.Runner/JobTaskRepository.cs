using System;
using TickTock.Core.Executions;
using TickTock.Core.Jobs;

namespace TickTock.Runner
{
    public class JobTaskRepository
    {
        public Func<Job, JobTask> New { get; set; }

        public Func<Job, JobExecution, JobTask> GetByJob { get; set; }
    }
}