using F2F.Sandbox;
using FluentAssertions;
using System;
using System.IO;
using System.IO.Compression;
using TickTock.Core.Blobs;
using TickTock.Core.Extensions;
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
            repository = BlobRepositoryFactory.Create(with =>
            {
                with.Location = sandbox.Directory;
            });
        }

        public class AddingBlob : BlobRepositoryTests
        {
            [Fact]
            public void ShouldGenerateNewGuid()
            {
                byte[] data = GetSampleZipFile();

                Blob blob = repository.New(data).GetBlob();

                blob.Identifier.Should().NotBeEmpty();
            }

            [Fact]
            public void NotFromTheZipFileShouldNotCreateBlob()
            {
                byte[] data = GetSampleUnknownFile();

                BlobCreation creation = repository.New(data);

                creation.Success.Should().BeFalse();
            }
        }

        public class RequestingBlob : BlobRepositoryTests
        {
            [Fact]
            public void ShouldReturnItsIdentifier()
            {
                Guid identifier = NewBlob().Identifier;

                Blob blob = repository.GetById(identifier);

                blob.Identifier.Should().Be(identifier);
            }

            [Fact]
            public void WhichIsNotAddedBlobShouldReturnNull()
            {
                Guid identifier = Guid.NewGuid();

                Blob blob = repository.GetById(identifier);

                blob.Should().BeNull();
            }
        }

        public class ReturnedBlob : BlobRepositoryTests
        {
            [Fact]
            public void ShouldReturnItsSize()
            {
                byte[] data;
                Blob blob = NewBlob(out data);

                blob.GetSize().Should().Be(data.Length);
            }

            [Fact]
            public void ShouldReturnItsHash()
            {
                byte[] data;
                Blob blob = NewBlob(out data);

                blob.GetHash().Should().Be(data.ToHash());
            }

            [Fact]
            public void ShouldReturnItsFiles()
            {
                Blob blob = NewBlob();

                blob.GetFiles().Should().ContainSingle(x =>
                    x.Name == "file" && x.Path == "file");
            }
        }

        public class Deploying : BlobRepositoryTests
        {
            [Fact]
            public void ShouldReturnPath()
            {
                Blob blob = NewBlob();
                BlobDeployment deployment;

                using (FileSandbox sandbox = new FileSandbox(new EmptyFileLocator()))
                {
                    deployment = blob.DeployTo(sandbox.Directory);
                    deployment.Path.Should().Be(sandbox.Directory);
                }
            }

            [Fact]
            public void ShouldReturnAllFiles()
            {
                Blob blob = NewBlob();
                BlobDeployment deployment;

                using (FileSandbox sandbox = new FileSandbox(new EmptyFileLocator()))
                {
                    deployment = blob.DeployTo(sandbox.Directory);
                    deployment.Files.Should().ContainSingle(x =>
                        x.Name == "file" && x.Path == Path.Combine(deployment.Path, "file"));
                }
            }
        }

        private Blob NewBlob()
        {
            byte[] data;
            return NewBlob(out data);
        }

        private Blob NewBlob(out byte[] data)
        {
            data = GetSampleZipFile();
            Blob added = repository.New(data).GetBlob();

            return added;
        }

        private static byte[] GetSampleZipFile()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Create))
                {
                    using (Stream entry = archive.CreateEntry("file").Open())
                    {
                        entry.Write(new byte[] { 0x01, 0x02, 0x03 }, 0, 3);
                    }
                }

                return stream.ToArray();
            }
        }

        private static byte[] GetSampleUnknownFile()
        {
            return new byte[] { 0x01, 0x02, 0x03 };
        }

        public void Dispose()
        {
            sandbox.Dispose();
        }
    }
}