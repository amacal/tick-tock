using F2F.Sandbox;
using FluentAssertions;
using System;
using TickTock.Core.Jobs;
using Xunit;

namespace TickTock.Core.Tests.Jobs
{
    public class JobRepositoryTests : IDisposable
    {
        private readonly FileSandbox sandbox;
        private readonly JobRepository repository;

        public JobRepositoryTests()
        {
            sandbox = new FileSandbox(new EmptyFileLocator());
            repository = JobRepositoryFactory.Create(with =>
            {
                with.Location = sandbox.Directory;
            });
        }

        public class AddingJob : JobRepositoryTests
        {
            [Fact]
            public void ShouldReturnGuid()
            {
                JobData data = Jobs.TickTockData;

                JobHeader header = repository.New(data);

                header.Identifier.Should().NotBeEmpty();
            }

            [Fact]
            public void ShouldReturnFirstVersion()
            {
                JobData data = Jobs.TickTockData;

                JobHeader header = repository.New(data);

                header.Version.Should().Be(1);
            }
        }

        [Fact]
        public void UpdatingJobShouldReturnSameGuid()
        {
            JobData data = Jobs.TickTockData;
            JobHeader created = repository.New(data);

            JobData upgrade = Jobs.TickTockDataV2;
            JobHeader header = repository.Update(created.Identifier, upgrade);

            header.Identifier.Should().Be(created.Identifier);
        }

        [Fact]
        public void UpdatingJobShouldReturnNextVersion()
        {
            JobData data = Jobs.TickTockData;
            JobHeader created = repository.New(data);

            JobData upgrade = Jobs.TickTockDataV2;
            JobHeader header = repository.Update(created.Identifier, upgrade);

            header.Version.Should().Be(created.Version + 1);
        }

        [Fact]
        public void RequestingAddedJobShouldReturnItsIdentifier()
        {
            JobData data = Jobs.TickTockData;
            JobHeader created = repository.New(data);

            Job job = repository.Single(with =>
            {
                with.Identifier = created.Identifier;
            });

            job.Header.Identifier.Should().Be(created.Identifier);
        }

        [Fact]
        public void RequestingAddedJobShouldReturnItsData()
        {
            JobData initial = Jobs.TickTockData;
            JobHeader created = repository.New(initial);

            Job job = repository.Single(with =>
            {
                with.Identifier = created.Identifier;
            });

            job.Extract(data =>
            {
                data.ShouldBeEquivalentTo(initial);
            });
        }

        [Fact]
        public void RequestingNotAddedJobShouldReturnNull()
        {
            Guid identifier = Guid.NewGuid();

            Job job = repository.Single(with =>
            {
                with.Identifier = identifier;
            });

            job.Should().BeNull();
        }

        [Fact]
        public void RequestingUpgradedJobShouldReturnNewestVersion()
        {
            JobData initial = Jobs.TickTockData;
            JobHeader created = repository.New(initial);

            JobData upgrade = Jobs.TickTockDataV2;
            JobHeader header = repository.Update(created.Identifier, upgrade);

            Job job = repository.Single(with =>
            {
                with.Identifier = created.Identifier;
                with.Version = header.Version;
            });

            job.Extract(data =>
            {
                data.ShouldBeEquivalentTo(upgrade);
            });
        }

        [Fact]
        public void RequestingUpgradedJobByVersionShouldReturnRequestedVersion()
        {
            JobData initial = Jobs.TickTockData;
            JobHeader created = repository.New(initial);

            JobData upgrade = Jobs.TickTockDataV2;
            JobHeader header = repository.Update(created.Identifier, upgrade);

            Job job = repository.Single(with =>
            {
                with.Identifier = created.Identifier;
                with.Version = created.Version;
            });

            job.Extract(data =>
            {
                data.ShouldBeEquivalentTo(initial);
            });
        }

        [Fact]
        public void RequestingJobByNotUpgradedVersionShouldReturnNull()
        {
            JobData data = Jobs.TickTockData;
            JobHeader created = repository.New(data);

            Job job = repository.Single(with =>
            {
                with.Identifier = created.Identifier;
                with.Version = created.Version + 1;
            });

            job.Should().BeNull();
        }

        public void Dispose()
        {
            sandbox.Dispose();
        }

        public static class Jobs
        {
            public static readonly JobData TickTockData = new JobData
            {
                Name = "tick-tock",
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod.",
                Executable = "tick-tock.exe",
                Blob = Guid.NewGuid()
            };

            public static readonly JobData TickTockDataV2 = new JobData
            {
                Name = "tick-tock",
                Description = "Lorem ipsum dolor sit amet.",
                Executable = "tick-tock.exe",
                Arguments = "--reprocess",
                Blob = Guid.NewGuid()
            };
        }
    }
}