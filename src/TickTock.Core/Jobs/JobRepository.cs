using System;

namespace TickTock.Core.Jobs
{
    public class JobRepository
    {
        public Func<Job[]> GetAll { get; set; }

        public Func<Guid, Job> GetById { get; set; }

        public Func<Guid, int, Job> GetByIdAndVersion { get; set; }

        public Func<JobData, JobHeader> Add { get; set; }

        public Func<Guid, JobData, JobHeader> Update { get; set; }
    }
}