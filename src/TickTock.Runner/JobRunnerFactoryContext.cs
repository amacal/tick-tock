using TickTock.Core.Executions;
using TickTock.Core.Jobs;

namespace TickTock.Runner
{
    public class JobRunnerFactoryContext
    {
        public JobRepository Jobs { get; set; }

        public JobExecutionRepository Executions { get; set; }

        public JobTaskRepository Tasks { get; set; }
    }
}