using TickTock.Core.Blobs;
using TickTock.Core.Executions;
using TickTock.Core.Jobs;

namespace TickTock.Runner
{
    public class JobTaskFactoryContext
    {
        public Job Job { get; set; }

        public BlobRepository Blobs { get; set; }

        public JobExecution Execution { get; set; }
    }
}