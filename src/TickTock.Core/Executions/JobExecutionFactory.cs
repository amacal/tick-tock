using System;
using System.IO;
using TickTock.Core.Blobs;
using TickTock.Core.Extensions;
using TickTock.Core.Jobs;

namespace TickTock.Core.Executions
{
    public static class JobExecutionFactory
    {
        public static JobExecution Build(Action<JobExecutionFactoryContext> with)
        {
            return with.Apply(context =>
            {
                return new JobExecution
                {
                    Identifier = context.Entry.Identifier,
                    Deploy = Deploy(context),
                    NextRun = NextRun(context),
                    Job = context.Entry.Job,
                    Progress = new JobExecutionProgress
                    {
                        GetStatus = GetStatus(context),
                        OnScheduled = OnScheduled(context),
                        OnStarted = OnStarted(context),
                        OnCompleted = OnCompleted(context),
                        OnFailed = OnFailed(context),
                        GetPid = GetPid(context)
                    },
                    Metrics = new JobExecutionMetrics
                    {
                        OnMemory = OnMemory(context),
                        OnProcessor = OnProcessor(context)
                    }
                };
            });
        }

        private static Func<Blob, BlobDeployment> Deploy(JobExecutionFactoryContext context)
        {
            return blob =>
            {
                string root = context.Entry.Path;
                string path = Path.Combine(root, "blob");

                return blob.DeployTo(path);
            };
        }

        private static Func<JobSchedule, DateTime?> NextRun(JobExecutionFactoryContext context)
        {
            return schedule =>
            {
                if (schedule == null)
                    return null;

                DateTime? lastExecutedAt = null;
                string root = context.Entry.Path;

                string path = Path.Combine(root, "metadata");
                string file = Path.Combine(path, ".completed");

                if (File.Exists(file))
                    lastExecutedAt = File.GetCreationTime(file);

                return schedule.Next(lastExecutedAt);
            };
        }

        private static Action OnScheduled(JobExecutionFactoryContext context)
        {
            return () =>
            {
                string root = context.Entry.Path;
                string path = Path.Combine(root, "metadata");
                string file = Path.Combine(path, ".scheduled");

                File.WriteAllBytes(file, new byte[0]);
            };
        }

        private static Action<int> OnStarted(JobExecutionFactoryContext context)
        {
            return pid =>
            {
                string root = context.Entry.Path;
                string path = Path.Combine(root, "metadata");
                string file = Path.Combine(path, ".pid");

                File.WriteAllText(file, pid.ToString());
            };
        }

        private static Action OnCompleted(JobExecutionFactoryContext context)
        {
            return () =>
            {
                string root = context.Entry.Path;
                string path = Path.Combine(root, "metadata");
                string file = Path.Combine(path, ".completed");

                File.WriteAllBytes(file, new byte[0]);
            };
        }

        private static Action<string> OnFailed(JobExecutionFactoryContext context)
        {
            return reason =>
            {
                string root = context.Entry.Path;
                string path = Path.Combine(root, "metadata");
                string file = Path.Combine(path, ".failed");

                File.WriteAllText(file, reason);
            };
        }

        private static Func<int> GetPid(JobExecutionFactoryContext context)
        {
            return () =>
            {
                string root = context.Entry.Path;
                string path = Path.Combine(root, "metadata");
                string file = Path.Combine(path, ".pid");

                return Int32.Parse(File.ReadAllText(file));
            };
        }

        private static Action<JobMemoryUsage> OnMemory(JobExecutionFactoryContext context)
        {
            return usage =>
            {
            };
        }

        private static Action<JobProcessorUsage> OnProcessor(JobExecutionFactoryContext context)
        {
            return usage =>
            {
            };
        }

        private static Func<JobExecutionStatus> GetStatus(JobExecutionFactoryContext context)
        {
            return () =>
            {
                string root = context.Entry.Path;
                string path = Path.Combine(root, "metadata");

                if (File.Exists(Path.Combine(path, ".completed")))
                    return JobExecutionStatus.Completed;

                if (File.Exists(Path.Combine(path, ".pid")))
                    return JobExecutionStatus.Running;

                if (File.Exists(Path.Combine(path, ".scheduled")))
                    return JobExecutionStatus.Pending;

                if (File.Exists(Path.Combine(path, ".failed")))
                    return JobExecutionStatus.Failed;

                return JobExecutionStatus.Idle;
            };
        }
    }
}