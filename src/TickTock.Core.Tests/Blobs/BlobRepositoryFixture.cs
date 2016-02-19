using F2F.Sandbox;
using FluentAssertions;
using System;
using TickTock.Core.Blobs;
using Xunit;

namespace TickTock.Core.Tests.Blobs
{
    public class BlobRepositoryFixture : IDisposable
    {
        private readonly FileSandbox sandbox;
        private readonly BlobRepository repository;

        public BlobRepositoryFixture()
        {
            this.sandbox = new FileSandbox(new EmptyFileLocator());
            this.repository = new BlobRepository(this.sandbox.Directory);
        }

        [Fact]
        public void AddingBlobShouldGenerateNewGuid()
        {
            byte[] data = { 0x01, 0x02, 0x03 };

            Guid identifier = repository.Add(data);

            identifier.Should().NotBeEmpty();
        }

        [Fact]
        public void RequestingAddedBlobShouldReturnItsIdentifier()
        {
            byte[] data = { 0x01, 0x02, 0x03 };
            Guid identifier = repository.Add(data);

            Blob blob = repository.GetById(identifier);

            Assert.Equal(identifier, blob.Identifier);
        }

        [Fact]
        public void RequestingAddedBlobShouldReturnItsSize()
        {
            byte[] data = { 0x01, 0x02, 0x03 };
            Guid identifier = repository.Add(data);

            Blob blob = repository.GetById(identifier);

            blob.Size.Should().Be(3);
        }

        [Fact]
        public void RequestingAddedBlobShouldReturnItsHash()
        {
            byte[] data = { 0x01, 0x02, 0x03 };
            Guid identifier = repository.Add(data);

            Blob blob = repository.GetById(identifier);

            blob.Hash.Should().Be("5289df737df57326fcdd22597afb1fac");
        }

        [Fact]
        public void RequestingAddedBlobShouldReturnItsContent()
        {
            byte[] data = { 0x01, 0x02, 0x03 };
            Guid identifier = repository.Add(data);

            byte[] content = repository.GetData(identifier);

            content.Should().Equal(data);
        }

        public void Dispose()
        {
            sandbox.Dispose();
        }
    }
}