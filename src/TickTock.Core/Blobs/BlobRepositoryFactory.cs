using System;
using System.IO;
using System.IO.Compression;
using TickTock.Core.Extensions;

namespace TickTock.Core.Blobs
{
    public static class BlobRepositoryFactory
    {
        public static BlobRepository Create(string location)
        {
            return new BlobRepository
            {
                GetById = GetById(location),
                Add = Add(location)
            };
        }

        private static Func<Guid, Blob> GetById(string location)
        {
            return identifier =>
            {
                string name = identifier.ToHex();
                string[] files = Directory.GetFiles(location, $"{name}-*");

                if (files.Length != 1)
                    return null;

                return Build(identifier, files[0]);
            };
        }

        public static Func<byte[], Blob> Add(string location)
        {
            return data =>
            {
                Guid identifier = Guid.NewGuid();
                string name = identifier.ToHex();

                string hash = data.ToHash();
                string path = Path.Combine(location, $"{name}-{hash}");

                File.WriteAllBytes(path, data);
                return Build(identifier, path);
            };
        }

        private static Blob Build(Guid identifier, string filename)
        {
            Blob blob = new Blob
            {
                Identifier = identifier
            };

            blob.GetHash = GetHash(blob, filename);
            blob.GetSize = GetSize(blob, filename);
            blob.DeployTo = DeployTo(blob, filename);

            return blob;
        }

        private static Func<string> GetHash(Blob blob, string path)
        {
            return () =>
            {
                return Path.GetFileName(path).Split('-')[1];
            };
        }

        private static Func<long> GetSize(Blob blob, string path)
        {
            return () =>
            {
                return new FileInfo(path).Length;
            };
        }

        private static Action<string> DeployTo(Blob blob, string path)
        {
            return destination =>
            {
                ZipFile.ExtractToDirectory(path, destination);
            };
        }
    }
}