﻿using System;
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

                foreach (Job job in context.Jobs.GetAll())
                {
                    JobExecution execution = context.Executions.GetByJob(job.Header).First();
                    if (execution.Progress.GetStatus() == JobExecutionStatus.Completed)
                    {
                        DateTime nextRun = execution.Schedule.Started.Add(job.Schedule.Interval);
                        if (nextRun < DateTime.Now)
                        {
                            task = context.Tasks.New(job);
                            task.Start();
                        }
                    }
                    else
                    {
                        task = context.Tasks.GetByJob(job, execution);
                        execution.Metrics.OnMemory(task.Statistics.GetMemory());
                        execution.Metrics.OnProcessor(task.Statistics.GetProcessor());
                    }
                }
            };
        }
    }
}