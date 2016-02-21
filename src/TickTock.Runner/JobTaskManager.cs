namespace TickTock.Runner
{
    public class JobTaskManager
    {
        public void Start(JobTask task)
        {
        }

        public void Stop(JobTask task)
        {
        }

        public bool IsWorking(JobTask task)
        {
            return false;
        }

        public JobRuntime GetRuntime(JobTask task)
        {
            return new JobRuntime
            {
                Task = task
            };
        }
    }
}