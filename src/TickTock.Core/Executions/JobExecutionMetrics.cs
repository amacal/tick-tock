using System;

namespace TickTock.Core.Executions
{
    public class JobExecutionMetrics
    {
        public Action<string[]> OnOutput { get; set; }

        public Action<string[]> OnError { get; set; }

        public Action<JobMemoryUsage> OnMemory { get; set; }

        public Action<JobProcessorUsage> OnProcessor { get; set; }
    }
}