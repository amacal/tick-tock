using System;

namespace TickTock.Runner
{
    public class JobTask
    {
        public Guid Identifier { get; set; }

        public JobTaskStatistics Statistics { get; set; }

        public Action Start { get; set; }
    }
}