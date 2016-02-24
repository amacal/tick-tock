using System;
using TickTock.Core.Jobs;

namespace TickTock.Core.Executions
{
    public class JobExecutionFileEntry
    {
        public string Path { get; set; }

        public Guid Identifier { get; set; }

        public JobHeader Job { get; set; }
    }
}