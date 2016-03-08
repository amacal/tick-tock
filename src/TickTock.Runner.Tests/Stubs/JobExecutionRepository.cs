using System;
using System.Collections.Generic;
using System.Linq;
using TickTock.Core.Executions;
using TickTock.Core.Jobs;

namespace TickTock.Runner.Tests.Stubs
{
    public class JobExecutionRepositoryStub : JobExecutionRepository
    {
        private readonly List<JobExecution> items;

        public JobExecutionRepositoryStub(Action<JobExecutionRepositoryConfigurer> with)
        {
            base.New = Add;
            base.GetById = GetById;

            items = new List<JobExecution>();
            with(new JobExecutionRepositoryConfigurer(items));
        }

        private new JobExecution Add(JobHeader job)
        {
            JobExecution execution = new JobExecution
            {
                Identifier = Guid.NewGuid(),
                Job = job,
            };

            items.Add(execution);
            return execution;
        }

        private new JobExecution GetById(Guid identifier)
        {
            return items.FirstOrDefault(x => x.Identifier == identifier);
        }
    }
}