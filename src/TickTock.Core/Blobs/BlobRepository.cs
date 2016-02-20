using System;
using System.IO;
using TickTock.Core.Extensions;

namespace TickTock.Core.Blobs
{
    public class BlobRepository : IBlobRepository
    {
        private readonly string location;

        public BlobRepository(string location)
        {
            this.location = location;
        }

        public Blob GetById(Guid identifier)
        {
            string name = identifier.ToHex();
            string[] files = Directory.GetFiles(location, $"{name}-*");

            if (files.Length != 1)
                return null;

            string[] parts = Path.GetFileName(files[0]).Split('-');
            FileInfo file = new FileInfo(files[0]);

            return new Blob
            {
                Identifier = identifier,
                Hash = parts[1],
                Size = file.Length
            };
        }

        public byte[] GetData(Guid identifier)
        {
            string name = identifier.ToHex();
            string[] files = Directory.GetFiles(location, $"{name}-*");

            return File.ReadAllBytes(files[0]);
        }

        public Guid Add(byte[] data)
        {
            Guid identifier = Guid.NewGuid();
            string name = identifier.ToHex();

            string hash = data.ToHash();
            string path = Path.Combine(location, $"{name}-{hash}");

            File.WriteAllBytes(path, data);
            return identifier;
        }
    }
}