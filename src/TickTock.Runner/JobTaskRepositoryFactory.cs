using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TickTock.Core.Blobs;
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
                return Build(context, context.Executions.Add(job.Header), job);
            };
        }

        private static Func<Job, JobExecution, JobTask> GetByJob(JobTaskRepositoryFactoryContext context)
        {
            return (job, execution) =>
            {
                return Build(context, execution, job);
            };
        }

        private static JobTask Build(JobTaskRepositoryFactoryContext context, JobExecution execution, Job job)
        {
            JobTask task = new JobTask
            {
                Identifier = execution.Identifier,
                Statistics = new JobTaskStatistics()
            };

            task.Start = Start(context, execution, task, job);
            task.Statistics.Publish = PublishStatistics(execution, task, job);

            return task;
        }

        private static Action Start(JobTaskRepositoryFactoryContext context, JobExecution execution, JobTask task, Job job)
        {
            return () =>
            {
                Blob blob = context.Blobs.GetById(job.Data.Blob);
                string executable = execution.GetPath(job.Data);

                ProcessStartInfo info = new ProcessStartInfo
                {
                    FileName = executable,
                    Arguments = job.Data.Arguments,
                    WorkingDirectory = Path.GetDirectoryName(executable),
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

                Process process = new Process
                {
                    StartInfo = info,
                    EnableRaisingEvents = true
                };

                process.Exited += (sender, args) =>
                {
                    execution.Progress.OnCompleted();
                    process.Dispose();
                };

                BlobDeployment deployment = execution.Deploy(blob);
                if (deployment == null)
                {
                    execution.Progress.OnFailed("Blob deployment failed.");
                    return;
                }

                execution.Progress.OnScheduled();

                try
                {
                    if (process.Start() == false)
                    {
                        execution.Progress.OnFailed("Process starting failed.");
                        return;
                    }
                }
                catch (Win32Exception ex)
                {
                    execution.Progress.OnFailed($"Process starting failed: {ex.Message}");
                    return;
                }

                execution.Progress.OnStarted(process.Id);
            };
        }

        private static Action PublishStatistics(JobExecution execution, JobTask task, Job job)
        {
            return () =>
            {
                OnProcess(execution, process =>
                {
                    execution.Metrics.OnMemory(new JobMemoryUsage
                    {
                        NonpagedSystemMemorySize = process.NonpagedSystemMemorySize64
                    });

                    execution.Metrics.OnProcessor(new JobProcessorUsage
                    {
                        TotalProcessorTime = process.TotalProcessorTime,
                        UserProcessorTime = process.UserProcessorTime
                    });
                });
            };
        }

        private static void OnProcess(JobExecution execution, Action<Process> callback)
        {
            int pid = execution.Progress.GetPid();
            Process process = Process.GetProcesses().FirstOrDefault(x => x.Id == pid);

            if (process != null)
                callback(process);
        }
    }
}