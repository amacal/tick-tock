﻿using F2F.Sandbox;
using FluentAssertions;
using System;
using TickTock.Core.Jobs;
using Xunit;

namespace TickTock.Core.Tests.Jobs
{
    public class JobRepositoryFixture : IDisposable
    {
        private readonly FileSandbox sandbox;
        private readonly JobRepository repository;

        public JobRepositoryFixture()
        {
            sandbox = new FileSandbox(new EmptyFileLocator());
            repository = new JobRepository(this.sandbox.Directory);
        }

        [Fact]
        public void AddingJobShouldReturnGuid()
        {
            JobData data = Jobs.TickTockData;

            JobHeader header = repository.Add(data);

            header.Identifier.Should().NotBeEmpty();
        }

        [Fact]
        public void AddingJobShouldReturnFirstVersion()
        {
            JobData data = Jobs.TickTockData;

            JobHeader header = repository.Add(data);

            header.Version.Should().Be(1);
        }

        [Fact]
        public void UpdatingJobShouldReturnSameGuid()
        {
            JobData data = Jobs.TickTockData;
            JobHeader created = repository.Add(data);

            JobData upgrade = Jobs.TickTockDataV2;
            JobHeader header = repository.Add(created.Identifier, upgrade);

            header.Identifier.Should().Be(created.Identifier);
        }

        [Fact]
        public void UpdatingJobShouldReturnNextVersion()
        {
            JobData data = Jobs.TickTockData;
            JobHeader created = repository.Add(data);

            JobData upgrade = Jobs.TickTockDataV2;
            JobHeader header = repository.Add(created.Identifier, upgrade);

            header.Version.Should().Be(created.Version + 1);
        }

        [Fact]
        public void RequestingAddedJobShouldReturnItsIdentifier()
        {
            JobData data = Jobs.TickTockData;
            JobHeader created = repository.Add(data);

            Job job = repository.GetById(created.Identifier);

            job.Header.Identifier.Should().Be(created.Identifier);
        }

        [Fact]
        public void RequestingAddedJobShouldReturnItsData()
        {
            JobData data = Jobs.TickTockData;
            JobHeader created = repository.Add(data);

            Job job = repository.GetById(created.Identifier);

            job.Data.ShouldBeEquivalentTo(data);
        }

        [Fact]
        public void RequestingNotAddedJobShouldReturnNull()
        {
            Guid identifier = Guid.NewGuid();

            Job job = repository.GetById(identifier);

            job.Should().BeNull();
        }

        [Fact]
        public void RequestingUpgradedJobShouldReturnNewestVersion()
        {
            JobData data = Jobs.TickTockData;
            JobHeader created = repository.Add(data);

            JobData upgrade = Jobs.TickTockDataV2;
            JobHeader header = repository.Add(created.Identifier, upgrade);

            Job job = repository.GetById(created.Identifier);

            job.Data.ShouldBeEquivalentTo(upgrade);
        }

        [Fact]
        public void RequestingUpgradedJobByVersionShouldReturnRequestedVersion()
        {
            JobData data = Jobs.TickTockData;
            JobHeader created = repository.Add(data);

            JobData upgrade = Jobs.TickTockDataV2;
            JobHeader header = repository.Add(created.Identifier, upgrade);

            Job job = repository.GetById(created.Identifier, created.Version);

            job.Data.ShouldBeEquivalentTo(data);
        }

        [Fact]
        public void RequestingJobByNotUpgradedVersionShouldReturnNull()
        {
            JobData data = Jobs.TickTockData;
            JobHeader created = repository.Add(data);

            Job job = repository.GetById(created.Identifier, created.Version + 1);

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
                Blob = Guid.NewGuid()
            };

            public static readonly JobData TickTockDataV2 = new JobData
            {
                Name = "tick-tock",
                Description = "Lorem ipsum dolor sit amet.",
                Blob = Guid.NewGuid()
            };
        }
    }
}