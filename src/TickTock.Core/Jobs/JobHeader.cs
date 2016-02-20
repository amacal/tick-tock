using System;

namespace TickTock.Core.Jobs
{
    public class JobHeader
    {
        public Guid Identifier { get; set; }

        public int Version { get; set; }
    }
}