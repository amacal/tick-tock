using System;
using TickTock.Core.Executions;

namespace TickTock.Runner
{
    public class JobTaskStatistics
    {
        public Func<JobMemoryUsage> GetMemory { get; set; }

        public Func<JobProcessorUsage> GetProcessor { get; set; }
    }
}