using FluentAssertions;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using TickTock.Core.Extensions;
using TickTock.Core.Jobs;
using TickTock.Gate.Modules;
using Xunit;

namespace TickTock.Gate.Tests.Modules
{
    [Collection("Nancy")]
    public class JobsModuleFixture : IDisposable
    {
        private readonly INancyBootstrapper bootstrapper;
        private readonly JobRepository repository;
        private readonly Browser browser;

        public JobsModuleFixture()
        {
            repository = new JobRepository(with =>
            {
                with.Job(Jobs.SampleHeader, Jobs.SampleData);
            });

            bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.Module<JobsModule>();
                with.Dependency<IJobRepository>(repository);
            });

            browser = new Browser(bootstrapper, with =>
            {
            });
        }

        [Fact]
        public void PostingNewJobShouldReturn200()
        {
            BrowserResponse response;
            PostJobRequest request = new PostJobRequest();

            response = browser.Post("/jobs", with =>
            {
                with.JsonBody(request);
            });

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public void PostingNewJobShouldReturnJson()
        {
            BrowserResponse response;
            PostJobRequest request = new PostJobRequest();

            response = browser.Post("/jobs", with =>
            {
                with.JsonBody(request);
            });

            response.ContentType.Should().StartWith("application/json");
        }

        [Fact]
        public void PostingNewJobShouldReturnItsIdentifier()
        {
            BrowserResponse response;
            PostJobRequest request = new PostJobRequest();

            response = browser.Post("/jobs", with =>
            {
                with.JsonBody(request);
            });

            response.Body.DeserializeJson<PostJobResponse>().id.Should().HaveLength(32);
        }

        [Fact]
        public void GettingNotExistingJobShouldReturn404()
        {
            string id = Guid.NewGuid().ToHex();

            BrowserResponse response = browser.Get($"/jobs/{id}", with =>
            {
            });

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public void GettingExistingJobShouldReturn200()
        {
            string id = Jobs.SampleHeader.Identifier.ToHex();

            BrowserResponse response = browser.Get($"/jobs/{id}", with =>
            {
            });

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public void GettingExistingJobShouldReturnJson()
        {
            string id = Jobs.SampleHeader.Identifier.ToHex();

            BrowserResponse response = browser.Get($"/jobs/{id}", with =>
            {
            });

            response.ContentType.Should().StartWith("application/json");
        }

        [Fact]
        public void GettingExistingJobShouldReturnsItsData()
        {
            string id = Jobs.SampleHeader.Identifier.ToHex();
            GetJobResponse data = new GetJobResponse
            {
                id = id,
                version = Jobs.SampleHeader.Version,
                name = Jobs.SampleData.Name,
                description = Jobs.SampleData.Description,
                blob = Jobs.SampleData.Blob.ToHex()
            };

            BrowserResponse response = browser.Get($"/jobs/{id}", with =>
            {
            });

            response.Body.DeserializeJson<GetJobResponse>().ShouldBeEquivalentTo(data);
        }

        public void Dispose()
        {
            bootstrapper.Dispose();
        }

        public class PostJobRequest
        {
            public string name;

            public string description;

            public string blob;
        }

        public class PostJobResponse
        {
            public string id;
        }

        public class GetJobResponse
        {
            public string id;

            public int version;

            public string name;

            public string description;

            public string blob;
        }

        public static class Jobs
        {
            public static readonly JobHeader SampleHeader = new JobHeader
            {
                Identifier = Guid.NewGuid(),
                Version = 1
            };

            public static readonly JobData SampleData = new JobData
            {
                Name = "tick-tock",
                Description = "Lorem ipsum dolor sit amet.",
                Blob = Guid.NewGuid()
            };
        }

        public class JobRepository : IJobRepository
        {
            private readonly Dictionary<JobHeader, JobData> items;

            public JobRepository(Action<JobRepositoryConfigurer> with)
            {
                items = new Dictionary<JobHeader, JobData>();
                with(new JobRepositoryConfigurer(items));
            }

            public JobHeader Add(JobData data)
            {
                JobHeader header = new JobHeader
                {
                    Identifier = Guid.NewGuid(),
                    Version = 1
                };

                items[header] = data;
                return header;
            }

            public Job GetById(Guid identifier)
            {
                return items
                    .Where(x => x.Key.Identifier == identifier)
                    .OrderByDescending(x => x.Key.Version)
                    .Select(x => new Job { Header = x.Key, Data = x.Value })
                    .FirstOrDefault();
            }
        }

        public class JobRepositoryConfigurer
        {
            private readonly Dictionary<JobHeader, JobData> items;

            public JobRepositoryConfigurer(Dictionary<JobHeader, JobData> items)
            {
                this.items = items;
            }

            public void Job(JobHeader header, JobData data)
            {
                items[header] = data;
            }
        }
    }
}