using System;
using TickTock.Core.Core;

namespace TickTock.Core.Jobs
{
    public class JobCriteria
    {
        public Criteria<Guid> Identifier { get; set; }

        public Criteria<int> Version { get; set; }
    }
}