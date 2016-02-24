using System.Collections.Generic;
using TickTock.Core.Executions;

namespace TickTock.Runner.Tests.Stubs
{
    public class JobExecutionRepositoryConfigurer
    {
        private readonly List<JobExecution> items;

        public JobExecutionRepositoryConfigurer(List<JobExecution> items)
        {
            this.items = items;
        }
    }
}