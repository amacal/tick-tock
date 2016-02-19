using System;

namespace TickTock.Core.Blobs
{
    public interface IBlobRepository
    {
        Guid Add(byte[] data);
    }
}