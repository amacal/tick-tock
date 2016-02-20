using System;

namespace TickTock.Core.Jobs
{
    public interface IJobRepository
    {
        Job GetById(Guid identifier);

        JobHeader Add(JobData data);
    }
}