using System;

namespace TickTock.Core.Jobs
{
    public class JobFileEntry
    {
        public string Path { get; set; }

        public Guid Identifier { get; set; }

        public int Version { get; set; }
    }
}