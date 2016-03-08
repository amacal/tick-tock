using System;
using TickTock.Core.Blobs;
using TickTock.Core.Jobs;

namespace TickTock.Core.Executions
{
    public class JobExecution
    {
        public Guid Identifier { get; set; }

        public JobHeader Job { get; set; }

        public JobExecutionMetrics Metrics { get; set; }

        public JobExecutionProgress Progress { get; set; }

        public Func<Blob, BlobDeployment> Deploy { get; set; }

        public Func<JobSchedule, DateTime?> NextRun { get; set; }
    }
}