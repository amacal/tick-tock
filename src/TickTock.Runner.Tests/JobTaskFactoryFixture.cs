using F2F.Sandbox;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using TickTock.Core.Blobs;
using TickTock.Core.Jobs;
using Xunit;

namespace TickTock.Runner.Tests
{
    public class JobTaskFactoryFixture : IDisposable
    {
        private readonly FileSandbox sandbox;
        private readonly JobTaskFactory factory;
        private readonly BlobRepository repository;

        public JobTaskFactoryFixture()
        {
            repository = new BlobRepository(with =>
            {
                with.Blob(Blobs.SampleId, blob =>
                {
                    blob.File("tick-tock.exe");
                });
            });

            sandbox = new FileSandbox(new EmptyFileLocator());
            factory = new JobTaskFactory(this.sandbox.Directory, repository);
        }

        [Fact]
        public void CreatingTaskShouldReturnPathWithinSandbox()
        {
            Job job = Jobs.Sample;

            JobTask task = factory.Create(job);

            task.Path.Should().StartWith(sandbox.Directory);
        }

        [Fact]
        public void CreatingTaskShouldReturnExecutable()
        {
            Job job = Jobs.Sample;

            JobTask task = factory.Create(job);

            task.Executable.Should().StartWith(task.Path);
            task.Executable.Should().EndWith(job.Data.Executable);
        }

        [Fact]
        public void CreatingTaskShouldReturnWorkingDirectory()
        {
            Job job = Jobs.Sample;

            JobTask task = factory.Create(job);

            task.WorkingDirectory.Should().StartWith(task.Path);
            task.WorkingDirectory.Should().Be(Path.GetDirectoryName(task.Executable));
        }

        [Fact]
        public void CreatingTaskShouldReturnArguments()
        {
            Job job = Jobs.Sample;

            JobTask task = factory.Create(job);

            task.Arguments.Should().Be(job.Data.Arguments);
        }

        public void Dispose()
        {
            sandbox.Dispose();
        }

        public static class Blobs
        {
            public static readonly Guid SampleId = Guid.NewGuid();
        }

        public static class Jobs
        {
            public static readonly Job Sample = new Job
            {
                Header = new JobHeader
                {
                    Identifier = Guid.NewGuid(),
                    Version = 1
                },
                Data = new JobData
                {
                    Name = "tick-tock",
                    Description = "Lorem ipsum dolor sit amet.",
                    Executable = "tick-tock.exe",
                    Arguments = "--dry-run",
                    Blob = Blobs.SampleId
                }
            };
        }

        public class BlobRepository : IBlobRepository
        {
            private readonly Dictionary<Guid, byte[]> items;

            public BlobRepository(Action<BlobRepositoryConfigurer> with)
            {
                items = new Dictionary<Guid, byte[]>();
                with(new BlobRepositoryConfigurer(items));
            }

            public Guid Add(byte[] data)
            {
                throw new NotImplementedException();
            }

            public Blob GetById(Guid identifier)
            {
                throw new NotImplementedException();
            }

            public byte[] GetData(Guid identifier)
            {
                return items[identifier];
            }
        }

        public class BlobRepositoryConfigurer
        {
            private readonly Dictionary<Guid, byte[]> items;

            public BlobRepositoryConfigurer(Dictionary<Guid, byte[]> items)
            {
                this.items = items;
            }

            public void Blob(Guid identifier, Action<BlobBuilder> with)
            {
                BlobBuilder builder = new BlobBuilder();

                with(builder);
                items[identifier] = builder.Build();
            }
        }

        public class BlobBuilder
        {
            private readonly Dictionary<string, byte[]> files;

            public BlobBuilder()
            {
                files = new Dictionary<string, byte[]>();
            }

            public void File(string path)
            {
                files[path] = new byte[0];
            }

            public byte[] Build()
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    using (ZipArchive archive = new ZipArchive(memory, ZipArchiveMode.Create, true))
                    {
                        foreach (string file in files.Keys)
                        {
                            archive.CreateEntry(file, CompressionLevel.Fastest);
                        }
                    }

                    return memory.ToArray();
                }
            }
        }
    }
}