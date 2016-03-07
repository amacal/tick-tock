using System;
using System.Linq;
using TickTock.Core.Executions;
using TickTock.Core.Extensions;
using TickTock.Core.Jobs;

namespace TickTock.Runner
{
    public static class JobRunnerFactory
    {
        public static JobRunner Create(Action<JobRunnerFactoryContext> with)
        {
            return with.Apply(context =>
            {
                return new JobRunner
                {
                    Run = Run(context)
                };
            });
        }

        private static Action Run(JobRunnerFactoryContext context)
        {
            return () =>
            {
                JobTask task;

                foreach (Job job in context.Jobs.All(with => { }))
                {
                    JobExecution execution = context.Executions.GetByJob(job.Header).FirstOrDefault();
                    if (execution == null || execution.Progress.GetStatus() == JobExecutionStatus.Idle)
                    {
                        task = context.Tasks.New(job);
                        task.Start();
                    }
                    else if (execution.Progress.GetStatus() == JobExecutionStatus.Completed)
                    {
                        if (execution.NextRun(job.Schedule) < DateTime.Now)
                        {
                            task = context.Tasks.New(job);
                            task.Start();
                        }
                    }
                    else if (execution.Progress.GetStatus() == JobExecutionStatus.Failed)
                    {
                        if (execution.NextRun(job.Schedule) < DateTime.Now)
                        {
                            task = context.Tasks.New(job);
                            task.Start();
                        }
                    }
                    else
                    {
                        task = context.Tasks.GetByJob(job, execution);
                        task.Statistics.Publish();
                    }
                }
            };
        }
    }
}