using System;
using TickTock.Core.Jobs;

namespace TickTock.Core.Executions
{
    public class JobExecutionRepository
    {
        public Func<Guid, JobExecution> GetById { get; set; }

        public Func<JobHeader, JobExecution[]> GetByJob { get; set; }

        public Func<JobHeader, JobExecution> New { get; set; }
    }
}