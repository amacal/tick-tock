using System;

namespace TickTock.Core.Jobs
{
    public class JobData
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Executable { get; set; }

        public string Arguments { get; set; }

        public Guid Blob { get; set; }
    }
}