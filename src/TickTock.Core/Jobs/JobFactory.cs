using Newtonsoft.Json;
using System;
using System.IO;
using TickTock.Core.Extensions;

namespace TickTock.Core.Jobs
{
    public static class JobFactory
    {
        public static Job Create(Action<JobFactoryContext> with)
        {
            return with.Apply(context =>
            {
                string content = File.ReadAllText(context.Entry.Path);
                JobHeader header = new JobHeader
                {
                    Identifier = context.Entry.Identifier,
                    Version = context.Entry.Version
                };

                Job job = new Job
                {
                    Header = header,
                    Data = JsonConvert.DeserializeObject<JobData>(content),
                    Schedule = new JobSchedule()
                };

                job.Schedule.Next = Next();

                return job;
            });
        }

        private static Func<DateTime?, DateTime?> Next()
        {
            return last =>
            {
                return DateTime.Now.AddHours(1);
            };
        }
    }
}