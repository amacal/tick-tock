using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TickTock.Core.Extensions;

namespace TickTock.Core.Jobs
{
    public static class JobRepositoryFactory
    {
        public static JobRepository Create(string location)
        {
            return new JobRepository
            {
                Add = Add(location),
                GetAll = GetAll(location),
                GetById = GetById(location),
                GetByIdAndVersion = GetByIdAndVersion(location),
                Update = Update(location)
            };
        }

        private static Func<Job[]> GetAll(string location)
        {
            return null;
        }

        private static Func<Guid, Job> GetById(string location)
        {
            return identifier =>
            {
                return GetByPredicate(location, identifier, i => true);
            };
        }

        private static Func<Guid, int, Job> GetByIdAndVersion(string location)
        {
            return (identifier, version) =>
            {
                return GetByPredicate(location, identifier, i => i.Version == version);
            };
        }

        private static Job GetByPredicate(string location, Guid identifier, Func<JobFileEntry, bool> predicate)
        {
            string name = identifier.ToHex();
            JobFileEntry current = GetInfo(location, name).FirstOrDefault(predicate);

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

        private static Func<JobData, JobHeader> Add(string location)
        {
            return data =>
            {
                return Add(location, Guid.NewGuid(), data);
            };
        }

        private static Func<Guid, JobData, JobHeader> Update(string location)
        {
            return (identifier, data) =>
            {
                return Add(location, identifier, data);
            };
        }

        private static JobHeader Add(string location, Guid identifier, JobData data)
        {
            int version = 1;
            string name = identifier.ToHex();

            JobFileEntry[] info = GetInfo(location, name);
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

        private static JobFileEntry[] GetInfo(string location, string identifier)
        {
            Regex regex = new Regex(@"\-(?<version>[0-9]+)$");
            string[] files = Directory.GetFiles(location, $"{identifier}-*");

            var query = from file in files
                        let match = regex.Match(file)
                        where match.Success
                        select new JobFileEntry
                        {
                            Path = file,
                            Version = Int32.Parse(match.Groups["version"].Value)
                        };

            return query.OrderByDescending(x => x.Version).ToArray();
        }
    }
}