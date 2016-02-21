namespace TickTock.Core.Jobs
{
    public class Job
    {
        public JobHeader Header { get; set; }

        public JobData Data { get; set; }

        public JobSchedule Schedule { get; set; }
    }
}