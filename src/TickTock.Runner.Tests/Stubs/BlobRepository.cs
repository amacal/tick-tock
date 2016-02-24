using System;
using System.Collections.Generic;
using TickTock.Core.Blobs;

namespace TickTock.Runner.Tests.Stubs
{
    public class BlobRepositoryStub : BlobRepository
    {
        private readonly Dictionary<Guid, byte[]> items;

        public BlobRepositoryStub(Action<BlobRepositoryConfigurer> with)
        {
            items = new Dictionary<Guid, byte[]>();
            with(new BlobRepositoryConfigurer(items));
        }
    }
}