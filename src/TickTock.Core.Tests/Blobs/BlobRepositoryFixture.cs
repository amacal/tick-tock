using F2F.Sandbox;
using System;
using TickTock.Core.Blobs;
using Xunit;

namespace TickTock.Core.Tests.Blobs
{
    public class BlobRepositoryFixture
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

            Assert.NotEqual(Guid.Empty, identifier);
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

            Assert.Equal(3, blob.Size);
        }

        [Fact]
        public void RequestingAddedBlobShouldReturnItsHash()
        {
            byte[] data = { 0x01, 0x02, 0x03 };
            Guid identifier = repository.Add(data);

            Blob blob = repository.GetById(identifier);

            Assert.Equal("5289df737df57326fcdd22597afb1fac", blob.Hash);
        }
    }
}