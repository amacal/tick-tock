using FluentAssertions;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Testing;
using System;
using System.Collections.Generic;
using TickTock.Core.Blobs;
using TickTock.Gate.Modules;
using Xunit;

namespace TickTock.Gate.Tests.Modules
{
    [Collection("Nancy")]
    public class BlobsModuleFixture
    {
        private readonly INancyBootstrapper bootstrapper;
        private readonly Browser browser;

        public BlobsModuleFixture()
        {
            bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.Module<BlobsModule>();
                with.Dependency<IBlobRepository>(new BlobRepository());
            });

            browser = new Browser(bootstrapper, with =>
            {
            });
        }

        [Fact]
        public void PostingBlobShouldReturn200()
        {
            BrowserResponse response;
            var model = new { };

            response = browser.Post("/blobs", with =>
            {
                with.JsonBody(model);
            });

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public void PostingBlobShouldReturnItsIdentifier()
        {
            BrowserResponse response = browser.Post("/blobs", with =>
            {
                with.Body("ABCD", "application/octet-stream");
            });

            response.Body.DeserializeJson<PostBlobResult>().id.Should().NotBeEmpty();
        }

        public class PostBlobResult
        {
            public Guid id;
        }

        public class BlobRepository : IBlobRepository
        {
            private readonly Dictionary<Guid, byte[]> items;

            public BlobRepository()
            {
                items = new Dictionary<Guid, byte[]>();
            }

            public Guid Add(byte[] data)
            {
                Guid id = Guid.NewGuid();
                items[id] = data;
                return id;
            }
        }
    }
}