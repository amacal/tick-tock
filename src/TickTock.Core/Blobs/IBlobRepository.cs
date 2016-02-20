using System;

namespace TickTock.Core.Blobs
{
    public interface IBlobRepository
    {
        Blob GetById(Guid identifier);

        byte[] GetData(Guid identifier);

        Guid Add(byte[] data);
    }
}