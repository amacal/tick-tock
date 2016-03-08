using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using TickTock.Core.Extensions;
using TickTock.Core.Jobs;

namespace TickTock.Core.Executions
{
    public static class JobExecutionRepositoryFactory
    {
        public static JobExecutionRepository Create(Action<JobExecutionRepositoryFactoryContext> with)
        {
            return with.Apply(context =>
            {
                return new JobExecutionRepository
                {
                    New = New(context),
                    GetById = GetById(context),
                    GetByJob = GetByJob(context)
                };
            });
        }

        private static Func<Guid, JobExecution> GetById(JobExecutionRepositoryFactoryContext context)
        {
            return identifier =>
            {
                foreach (JobExecutionFileEntry entry in Query(context.Location))
                {
                    if (entry.Identifier == identifier)
                    {
                        return JobExecutionFactory.Build(with =>
                        {
                            with.Entry = entry;
                        });
                    }
                }

                return null;
            };
        }

        private static Func<JobHeader, JobExecution[]> GetByJob(JobExecutionRepositoryFactoryContext context)
        {
            return job =>
            {
                List<JobExecution> executions = new List<JobExecution>();

                foreach (JobExecutionFileEntry entry in Query(context.Location))
                {
                    if (entry.Job == job)
                    {
                        executions.Add(JobExecutionFactory.Build(with =>
                        {
                            with.Entry = entry;
                        }));
                    }
                }

                return executions.ToArray();
            };
        }

        private static Func<JobHeader, JobExecution> New(JobExecutionRepositoryFactoryContext context)
        {
            return job =>
            {
                Guid identifier = Guid.NewGuid();
                string prefix = identifier.ToHex();
                string name = job.Identifier.ToHex();
                int version = job.Version;

                string root = Path.Combine(context.Location, $"{prefix}-{name}-{version}");
                string blob = Path.Combine(root, "blob");
                string metadata = Path.Combine(root, "metadata");

                Directory.CreateDirectory(root);
                Directory.CreateDirectory(blob);
                Directory.CreateDirectory(metadata);

                return JobExecutionFactory.Build(with =>
                {
                    with.Entry = new JobExecutionFileEntry
                    {
                        Job = job,
                        Path = root,
                        Identifier = identifier
                    };
                });
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
    }
}