using Nancy.Hosting.Self;
using System;
using System.IO;
using System.Threading;
using TickTock.Core.Blobs;
using TickTock.Core.Executions;
using TickTock.Core.Jobs;
using TickTock.Runner;

namespace TickTock.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            const string path = @"d:\\tick-tock";

            BlobRepository blobs = BlobRepositoryFactory.Create(Path.Combine(path, "blobs"));
            JobExecutionRepository executions = JobExecutionRepositoryFactory.Create(Path.Combine(path, "executions"));
            JobRepository jobs = JobRepositoryFactory.Create(Path.Combine(path, "jobs"));

            JobTaskRepository tasks = JobTaskRepositoryFactory.Create(with =>
            {
                with.Blobs = blobs;
                with.Executions = executions;
            });

            JobRunner runner = JobRunnerFactory.Create(with =>
            {
                with.Executions = executions;
                with.Jobs = jobs;
                with.Tasks = tasks;
            });

            Bootstrapper bootstrapper = new Bootstrapper(path);
            Uri uri = new Uri("http://localhost:1234");

            using (var host = new NancyHost(bootstrapper, uri))
            {
                host.Start();

                while (true)
                {
                    runner.Run();
                    Thread.Sleep(TimeSpan.FromSeconds(10));
                }
            }
        }
    }
}