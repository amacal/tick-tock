﻿using System;
using TickTock.Core.Blobs;
using TickTock.Core.Jobs;

namespace TickTock.Core.Executions
{
    public class JobExecution
    {
        public Guid Identifier { get; set; }

        public JobHeader Job { get; set; }

        public JobExecutionMetrics Metrics { get; set; }

        public JobExecutionSchedule Schedule { get; set; }

        public JobExecutionProgress Progress { get; set; }

        public Action<Blob> Deploy { get; set; }
    }
}