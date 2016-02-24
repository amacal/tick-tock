using F2F.Sandbox;
using FluentAssertions;
using System;
using TickTock.Core.Blobs;
using Xunit;

namespace TickTock.Core.Tests.Blobs
{
    public class BlobRepositoryTests : IDisposable
    {
        private readonly FileSandbox sandbox;
        private readonly BlobRepository repository;

        public BlobRepositoryTests()
        {
            sandbox = new FileSandbox(new EmptyFileLocator());
            repository = BlobRepositoryFactory.Create(sandbox.Directory);
        }

        [Fact]
        public void AddingBlobShouldGenerateNewGuid()
        {
            byte[] data = { 0x01, 0x02, 0x03 };

            Blob blob = repository.Add(data);

            blob.Identifier.Should().NotBeEmpty();
        }

        [Fact]
        public void RequestingNotAddedBlobShouldReturnNull()
        {
            Guid identifier = Guid.NewGuid();

            Blob blob = repository.GetById(identifier);

            blob.Should().BeNull();
        }

        [Fact]
        public void RequestingAddedBlobShouldReturnItsIdentifier()
        {
            byte[] data = { 0x01, 0x02, 0x03 };
            Blob added = repository.Add(data);

            Blob blob = repository.GetById(added.Identifier);

            blob.Identifier.Should().Be(added.Identifier);
        }

        [Fact]
        public void RequestingAddedBlobShouldReturnItsSize()
        {
            byte[] data = { 0x01, 0x02, 0x03 };
            Blob added = repository.Add(data);

            Blob blob = repository.GetById(added.Identifier);

            blob.GetSize().Should().Be(3);
        }

        [Fact]
        public void RequestingAddedBlobShouldReturnItsHash()
        {
            byte[] data = { 0x01, 0x02, 0x03 };
            Blob added = repository.Add(data);

            Blob blob = repository.GetById(added.Identifier);

            blob.GetHash().Should().Be("5289df737df57326fcdd22597afb1fac");
        }

        public void Dispose()
        {
            sandbox.Dispose();
        }
    }
}