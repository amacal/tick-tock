using System;

namespace TickTock.Core.Executions
{
    public class JobProcessorUsage
    {
        public TimeSpan TotalProcessorTime { get; set; }

        public TimeSpan UserProcessorTime { get; set; }
    }
}