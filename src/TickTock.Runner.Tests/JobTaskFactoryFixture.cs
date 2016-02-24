//using F2F.Sandbox;
//using FluentAssertions;
//using System;
//using System.IO;
//using TickTock.Core.Jobs;
//using TickTock.Runner.Tests.Stubs;
//using Xunit;

//namespace TickTock.Runner.Tests
//{
//    public class JobTaskFactoryFixture : IDisposable
//    {
//        private readonly FileSandbox sandbox;
//        private readonly JobTaskFactory factory;
//        private readonly BlobRepository blobs;

//        public JobTaskFactoryFixture()
//        {
//            blobs = new BlobRepository(with =>
//            {
//                with.Blob(Blobs.SampleId, blob =>
//                {
//                    blob.File("tick-tock.exe");
//                });
//            });

//            sandbox = new FileSandbox(new EmptyFileLocator());
//            factory = new JobTaskFactory(sandbox.Directory, blobs);
//        }

//        [Fact]
//        public void CreatingTaskShouldReturnPathWithinSandbox()
//        {
//            Job job = Jobs.Sample;

//            JobTask task = factory.Create(job);

//            task.Path.Should().StartWith(sandbox.Directory);
//        }

//        [Fact]
//        public void CreatingTaskShouldReturnExecutable()
//        {
//            Job job = Jobs.Sample;

//            JobTask task = factory.Create(job);

//            task.Executable.Should().StartWith(task.Path);
//            task.Executable.Should().EndWith(job.Data.Executable);
//        }

//        [Fact]
//        public void CreatingTaskShouldReturnWorkingDirectory()
//        {
//            Job job = Jobs.Sample;

//            JobTask task = factory.Create(job);

//            task.WorkingDirectory.Should().StartWith(task.Path);
//            task.WorkingDirectory.Should().Be(Path.GetDirectoryName(task.Executable));
//        }

//        [Fact]
//        public void CreatingTaskShouldReturnArguments()
//        {
//            Job job = Jobs.Sample;

//            JobTask task = factory.Create(job);

//            task.Arguments.Should().Be(job.Data.Arguments);
//        }

//        public void Dispose()
//        {
//            sandbox.Dispose();
//        }

//        public static class Blobs
//        {
//            public static readonly Guid SampleId = Guid.NewGuid();
//        }

//        public static class Jobs
//        {
//            public static readonly Job Sample = new Job
//            {
//                Header = new JobHeader
//                {
//                    Identifier = Guid.NewGuid(),
//                    Version = 1
//                },
//                Data = new JobData
//                {
//                    Name = "tick-tock",
//                    Description = "Lorem ipsum dolor sit amet.",
//                    Executable = "tick-tock.exe",
//                    Arguments = "--dry-run",
//                    Blob = Blobs.SampleId
//                }
//            };
//        }
//    }
//}