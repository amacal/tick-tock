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
                return new Job
                {
                    Extract = Extract(context),
                    Header = new JobHeader
                    {
                        Identifier = context.Entry.Identifier,
                        Version = context.Entry.Version
                    },
                    Schedule = new JobSchedule
                    {
                        Next = Next()
                    }
                };
            });
        }

        private static Action<Action<JobData>> Extract(JobFactoryContext context)
        {
            return callback =>
            {
                string content = File.ReadAllText(context.Entry.Path);
                JobData data = JsonConvert.DeserializeObject<JobData>(content);

                callback(data);
            };
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