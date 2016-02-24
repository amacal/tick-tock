using TickTock.Core.Blobs;
using TickTock.Core.Executions;

namespace TickTock.Runner
{
    public class JobTaskRepositoryFactoryContext
    {
        public BlobRepository Blobs { get; set; }

        public JobExecutionRepository Executions { get; set; }
    }
}