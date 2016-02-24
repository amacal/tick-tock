using System;
using System.Collections.Generic;

namespace TickTock.Runner.Tests.Stubs
{
    public class BlobRepositoryConfigurer
    {
        private readonly Dictionary<Guid, byte[]> items;

        public BlobRepositoryConfigurer(Dictionary<Guid, byte[]> items)
        {
            this.items = items;
        }

        public void Blob(Guid identifier, Action<BlobBuilder> with)
        {
            BlobBuilder builder = new BlobBuilder();

            with(builder);
            items[identifier] = builder.Build();
        }
    }
}