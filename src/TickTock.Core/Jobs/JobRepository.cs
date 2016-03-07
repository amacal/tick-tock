using System;

namespace TickTock.Core.Jobs
{
    public class JobRepository
    {
        public Func<Action<JobCriteria>, Job[]> All { get; set; }

        public Func<Action<JobCriteria>, Job> Single { get; set; }

        public Func<JobData, JobHeader> Add { get; set; }

        public Func<Guid, JobData, JobHeader> Update { get; set; }
    }
}