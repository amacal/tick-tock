using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TickTock.Core.Core;
using TickTock.Core.Extensions;

namespace TickTock.Core.Jobs
{
    public static class JobRepositoryFactory
    {
        public static JobRepository Create(Action<JobRepositoryFactoryContext> with)
        {
            return with.Apply(context =>
            {
                return new JobRepository
                {
                    New = New(context),
                    All = All(context),
                    Single = Single(context),
                    Update = Update(context)
                };
            });
        }

        private static Func<Action<JobCriteria>, Job[]> All(JobRepositoryFactoryContext context)
        {
            return callback => Get(context, callback).ToArray();
        }

        private static Func<Action<JobCriteria>, Job> Single(JobRepositoryFactoryContext context)
        {
            return callback => Get(context, callback).SingleOrDefault();
        }

        private static IEnumerable<Job> Get(JobRepositoryFactoryContext context, Action<JobCriteria> callback)
        {
            JobCriteria criteria = new JobCriteria
            {
                Identifier = Criteria<Guid>.Nothing,
                Version = Criteria<int>.Nothing
            };

            Func<JobFileEntry, bool> predicate = entry =>
            {
                return criteria.Identifier.Is(entry.Identifier)
                    && criteria.Version.Is(entry.Version);
            };

            callback(criteria);

            foreach (JobFileEntry entry in GetInfo(context.Location, predicate))
            {
                yield return JobFactory.Create(with =>
                {
                    with.Entry = entry;
                });
            }
        }

        private static Func<JobData, JobHeader> New(JobRepositoryFactoryContext context)
        {
            return data =>
            {
                return AddOrUpdate(context.Location, Guid.NewGuid(), data);
            };
        }

        private static Func<Guid, JobData, JobHeader> Update(JobRepositoryFactoryContext context)
        {
            return (identifier, data) =>
            {
                return AddOrUpdate(context.Location, identifier, data);
            };
        }

        private static JobHeader AddOrUpdate(string location, Guid identifier, JobData data)
        {
            int version = 1;
            string name = identifier.ToHex();

            JobFileEntry[] info = GetInfo(location, x => x.Identifier == identifier);
            JobFileEntry current = info.FirstOrDefault();

            if (current != null)
                version = current.Version + 1;

            string path = Path.Combine(location, $"{name}-{version}");
            JobHeader header = new JobHeader
            {
                Identifier = identifier,
                Version = version
            };

            File.WriteAllText(path, JsonConvert.SerializeObject(data));
            return header;
        }

        private static JobFileEntry[] GetInfo(string location, Func<JobFileEntry, bool> predicate)
        {
            Regex regex = new Regex(@"(?<identifier>[a-z0-9]{32})\-(?<version>[0-9]+)$");
            string[] files = Directory.GetFiles(location, "*-*");

            var query = from file in files
                        let match = regex.Match(file)
                        where match.Success
                        select new JobFileEntry
                        {
                            Path = file,
                            Identifier = Guid.Parse(match.Groups["identifier"].Value),
                            Version = Int32.Parse(match.Groups["version"].Value)
                        };

            return query.Where(predicate).OrderByDescending(x => x.Version).ToArray();
        }
    }
}