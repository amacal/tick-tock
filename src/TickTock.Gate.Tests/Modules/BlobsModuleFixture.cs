using FluentAssertions;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Testing;
using System;
using System.Collections.Generic;
using System.IO;
using TickTock.Core.Blobs;
using TickTock.Core.Extensions;
using TickTock.Gate.Modules;
using Xunit;

namespace TickTock.Gate.Tests.Modules
{
    [Collection("Nancy")]
    public class BlobsModuleFixture : IDisposable
    {
        private readonly INancyBootstrapper bootstrapper;
        private readonly BlobRepository repository;
        private readonly Browser browser;

        public BlobsModuleFixture()
        {
            repository = new BlobRepository(with =>
            {
                with.Blob(Blobs.SampleId, Blobs.SampleData);
            });

            bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.Module<BlobsModule>();
                with.Dependency<IBlobRepository>(repository);
            });

            browser = new Browser(bootstrapper, with =>
            {
            });
        }

        [Fact]
        public void PostingNewBlobShouldReturn200()
        {
            BrowserResponse response;

            response = browser.Post("/blobs", with =>
            {
                with.Body("ABCD", "application/octet-stream");
            });

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public void PostingNewBlobShouldReturnJson()
        {
            BrowserResponse response;

            response = browser.Post("/blobs", with =>
            {
                with.Body("ABCD", "application/octet-stream");
            });

            response.ContentType.Should().StartWith("application/json");
        }

        [Fact]
        public void PostingNewBlobShouldReturnItsIdentifier()
        {
            BrowserResponse response = browser.Post("/blobs", with =>
            {
                with.Body("ABCD", "application/octet-stream");
            });

            response.Body.DeserializeJson<PostBlobResult>().id.Should().HaveLength(32);
        }

        [Fact]
        public void GettingExistingBlobShouldReturn200()
        {
            string id = Blobs.SampleId;

            BrowserResponse response = browser.Get($"/blobs/{id}", with =>
            {
            });

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public void GettingExistingBlobShouldReturnApplicationJson()
        {
            string id = Blobs.SampleId;

            BrowserResponse response = browser.Get($"/blobs/{id}", with =>
            {
            });

            response.ContentType.Should().StartWith("application/json");
        }

        [Fact]
        public void GettingExistingBlobShouldReturnItsSize()
        {
            int size = Blobs.SampleData.Length;
            string id = Blobs.SampleId;

            BrowserResponse response = browser.Get($"/blobs/{id}", with =>
            {
            });

            response.Body.DeserializeJson<GetBlobResult>().size.Should().Be(size);
        }

        [Fact]
        public void GettingExistingBlobShouldReturnItsHash()
        {
            string hash = Blobs.SampleHash;
            string id = Blobs.SampleId;

            BrowserResponse response = browser.Get($"/blobs/{id}", with =>
            {
            });

            response.Body.DeserializeJson<GetBlobResult>().hash.Should().Be(hash);
        }

        [Fact]
        public void GettingNotExistingBlobShouldReturn404()
        {
            string id = "5129ac3c07e346269f55cbfb086f3e59";

            BrowserResponse response = browser.Get($"/blobs/{id}", with =>
            {
            });

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public void GettingExistingBlobDataShouldReturn200()
        {
            string id = Blobs.SampleId;

            BrowserResponse response = browser.Get($"/blobs/{id}/data", with =>
            {
            });

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public void GettingExistingBlobDataShouldReturnApplicationOctetStream()
        {
            string id = Blobs.SampleId;

            BrowserResponse response = browser.Get($"/blobs/{id}/data", with =>
            {
            });

            response.ContentType.Should().Be("application/octet-stream");
        }

        [Fact]
        public void GettingExistingBlobDataShouldReturnItsContent()
        {
            byte[] data = Blobs.SampleData;
            string id = Blobs.SampleId;

            BrowserResponse response = browser.Get($"/blobs/{id}/data", with =>
            {
            });

            using (Stream stream = response.Body.AsStream())
            using (MemoryStream memory = new MemoryStream())
            {
                stream.CopyTo(memory);
                memory.ToArray().Should().Equal(data);
            }
        }

        [Fact]
        public void GettingNotExistingBlobDataShouldReturn404()
        {
            string id = "5129ac3c07e346269f55cbfb086f3e59";

            BrowserResponse response = browser.Get($"/blobs/{id}/data", with =>
            {
            });

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        public void Dispose()
        {
            bootstrapper.Dispose();
        }

        public class PostBlobResult
        {
            public string id;
        }

        public class GetBlobResult
        {
            public int size;

            public string hash;
        }

        public static class Blobs
        {
            public static readonly string SampleId = "782ff7698fff4627b8c9d2a0fdf6e466";

            public static readonly byte[] SampleData = new byte[] { 0x01, 0x02, 0x03 };

            public static readonly string SampleHash = SampleData.ToHash();
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
                Guid id = Guid.NewGuid();
                items[id] = data;
                return id;
            }

            public Blob GetById(Guid identifier)
            {
                byte[] content;
                items.TryGetValue(identifier, out content);

                if (content == null)
                    return null;

                return new Blob
                {
                    Identifier = identifier,
                    Size = content.Length,
                    Hash = content.ToHash()
                };
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

            public void Blob(string identifier, byte[] data)
            {
                items[Guid.Parse(identifier)] = data;
            }
        }
    }
}