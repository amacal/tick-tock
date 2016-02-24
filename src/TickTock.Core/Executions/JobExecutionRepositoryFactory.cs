using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using TickTock.Core.Blobs;
using TickTock.Core.Extensions;
using TickTock.Core.Jobs;

namespace TickTock.Core.Executions
{
    public static class JobExecutionRepositoryFactory
    {
        public static JobExecutionRepository Create(string location)
        {
            return new JobExecutionRepository
            {
                Add = Add(location),
                GetById = GetById(location),
                GetByJob = GetByJob(location)
            };
        }

        private static Func<Guid, JobExecution> GetById(string location)
        {
            return identifier =>
            {
                foreach (JobExecutionFileEntry entry in Query(location))
                {
                    if (entry.Identifier == identifier)
                    {
                        return Build(location, entry.Identifier, entry.Job);
                    }
                }

                return null;
            };
        }

        private static Func<JobHeader, JobExecution[]> GetByJob(string location)
        {
            return job =>
            {
                List<JobExecution> executions = new List<JobExecution>();

                foreach (JobExecutionFileEntry entry in Query(location))
                {
                    if (entry.Job == job)
                    {
                        executions.Add(Build(location, entry.Identifier, entry.Job));
                    }
                }

                return executions.ToArray();
            };
        }

        private static Func<JobHeader, JobExecution> Add(string location)
        {
            return job =>
            {
                Guid identifier = Guid.NewGuid();
                string name = identifier.ToHex();
                string prefix = job.Identifier.ToHex();
                int version = job.Version;

                string root = Path.Combine(location, $"{prefix}-{name}-{version}");
                string blob = Path.Combine(root, "blob");
                string metadata = Path.Combine(root, "metadata");

                Directory.CreateDirectory(root);
                Directory.CreateDirectory(blob);
                Directory.CreateDirectory(metadata);

                return Build(location, identifier, job);
            };
        }

        private static IEnumerable<JobExecutionFileEntry> Query(string location)
        {
            Regex regex = new Regex("(?<id>[0-9a-z]{32})-(?<job>[0-9a-z]{32})-(?<version>[0-9]+)");

            foreach (string path in Directory.GetDirectories(location, "*-*"))
            {
                string name = Path.GetFileName(path);
                Match match = regex.Match(name);

                if (match.Success)
                {
                    yield return new JobExecutionFileEntry
                    {
                        Path = path,
                        Identifier = Guid.Parse(match.Groups["id"].Value),
                        Job = new JobHeader
                        {
                            Identifier = Guid.Parse(match.Groups["job"].Value),
                            Version = Int32.Parse(match.Groups["version"].Value)
                        }
                    };
                }
            }
        }

        private static JobExecution Build(string location, Guid identifier, JobHeader header)
        {
            JobExecution execution = new JobExecution
            {
                Identifier = identifier,
                Job = header,
                Progress = new JobExecutionProgress()
            };

            execution.Deploy = Deploy(location, execution);
            execution.Progress.GetStatus = GetStatus(location, execution);
            execution.Progress.OnScheduled = OnScheduled(location, execution);
            execution.Progress.OnStarted = OnStarted(location, execution);
            execution.Progress.OnCompleted = OnCompleted(location, execution);

            return execution;
        }

        private static Action<Blob> Deploy(string location, JobExecution execution)
        {
            return blob =>
            {
                string root = GetRootPath(location, execution);
                string path = Path.Combine(root, "blob");

                blob.DeployTo(path);
            };
        }

        private static Action OnScheduled(string location, JobExecution execution)
        {
            return () =>
            {
                string root = GetRootPath(location, execution);
                string path = Path.Combine(root, "metadata");
                string file = Path.Combine(path, ".scheduled");

                File.WriteAllBytes(file, new byte[0]);
            };
        }

        private static Action<int> OnStarted(string location, JobExecution execution)
        {
            return pid =>
            {
                string root = GetRootPath(location, execution);
                string path = Path.Combine(root, "metadata");
                string file = Path.Combine(path, ".pid");

                File.WriteAllText(file, pid.ToString());
            };
        }

        private static Action OnCompleted(string location, JobExecution execution)
        {
            return () =>
            {
                string root = GetRootPath(location, execution);
                string path = Path.Combine(root, "metadata");
                string file = Path.Combine(path, ".completed");

                File.WriteAllBytes(file, new byte[0]);
            };
        }

        private static Func<JobExecutionStatus> GetStatus(string location, JobExecution execution)
        {
            return () =>
            {
                string root = GetRootPath(location, execution);
                string path = Path.Combine(root, "metadata");

                if (File.Exists(Path.Combine(path, ".completed")))
                    return JobExecutionStatus.Completed;

                if (File.Exists(Path.Combine(path, ".pid")))
                    return JobExecutionStatus.Running;

                if (File.Exists(Path.Combine(path, ".scheduled")))
                    return JobExecutionStatus.Pending;

                return JobExecutionStatus.Idle;
            };
        }

        private static string GetRootPath(string location, JobExecution execution)
        {
            string name = execution.Identifier.ToHex();
            string prefix = execution.Job.Identifier.ToHex();
            int version = execution.Job.Version;

            return Path.Combine(location, $"{prefix}-{name}-{version}");
        }
    }
}