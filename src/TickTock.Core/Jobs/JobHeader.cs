using System;

namespace TickTock.Core.Jobs
{
    public class JobHeader
    {
        public Guid Identifier { get; set; }

        public int Version { get; set; }

        public static bool operator ==(JobHeader left, JobHeader right)
        {
            return left.Identifier == right.Identifier && left.Version == right.Version;
        }

        public static bool operator !=(JobHeader left, JobHeader right)
        {
            return left.Identifier != right.Identifier || left.Version != right.Version;
        }
    }
}