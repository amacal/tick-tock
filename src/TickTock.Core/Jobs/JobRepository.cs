using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TickTock.Core.Extensions;

namespace TickTock.Core.Jobs
{
    public class JobRepository
    {
        private readonly string location;

        public JobRepository(string location)
        {
            this.location = location;
        }

        public Job GetById(Guid identifier)
        {
            return GetByPredicate(identifier, i => true);
        }

        public Job GetById(Guid identifier, int version)
        {
            return GetByPredicate(identifier, i => i.Version == version);
        }

        private Job GetByPredicate(Guid identifier, Func<JobInfo, bool> predicate)
        {
            string name = identifier.ToHex();
            JobInfo current = GetInfo(name).FirstOrDefault(predicate);

            if (current == null)
                return null;

            string content = File.ReadAllText(current.Path);
            JobHeader header = new JobHeader
            {
                Identifier = identifier,
                Version = current.Version
            };

            return new Job
            {
                Header = header,
                Data = JsonConvert.DeserializeObject<JobData>(content)
            };
        }

        public JobHeader Add(JobData data)
        {
            return Add(Guid.NewGuid(), data);
        }

        public JobHeader Add(Guid identifier, JobData data)
        {
            int version = 1;
            string name = identifier.ToHex();

            JobInfo[] info = GetInfo(name);
            JobInfo current = info.FirstOrDefault();

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

        private JobInfo[] GetInfo(string identifier)
        {
            Regex regex = new Regex(@"\-(?<version>[0-9]+)$");
            string[] files = Directory.GetFiles(location, $"{identifier}-*");

            var query = from file in files
                        let match = regex.Match(file)
                        where match.Success
                        select new JobInfo
                        {
                            Path = file,
                            Version = Int32.Parse(match.Groups["version"].Value)
                        };

            return query.OrderByDescending(x => x.Version).ToArray();
        }
    }
}