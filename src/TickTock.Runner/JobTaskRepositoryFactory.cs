using System;
using TickTock.Core.Executions;
using TickTock.Core.Extensions;
using TickTock.Core.Jobs;

namespace TickTock.Runner
{
    public static class JobTaskRepositoryFactory
    {
        public static JobTaskRepository Create(Action<JobTaskRepositoryFactoryContext> with)
        {
            return with.Apply(context =>
            {
                return new JobTaskRepository
                {
                    New = New(context),
                    GetByJob = GetByJob(context)
                };
            });
        }

        private static Func<Job, JobTask> New(JobTaskRepositoryFactoryContext context)
        {
            return job =>
            {
                return JobTaskFactory.Build(with =>
                {
                    with.Job = job;
                    with.Blobs = context.Blobs;
                    with.Execution = context.Executions.New(job.Header);
                });
            };
        }

        private static Func<Job, JobExecution, JobTask> GetByJob(JobTaskRepositoryFactoryContext context)
        {
            return (job, execution) =>
            {
                return JobTaskFactory.Build(with =>
                {
                    with.Job = job;
                    with.Blobs = context.Blobs;
                    with.Execution = execution;
                });
            };
        }
    }
}