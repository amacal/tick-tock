using System;
using System.IO;
using System.IO.Compression;
using TickTock.Core.Extensions;

namespace TickTock.Core.Blobs
{
    public static class BlobRepositoryFactory
    {
        public static BlobRepository Create(Action<BlobRepositoryFactoryContext> with)
        {
            return with.Apply(context =>
            {
                return new BlobRepository
                {
                    GetById = GetById(context),
                    New = New(context)
                };
            });
        }

        private static Func<Guid, Blob> GetById(BlobRepositoryFactoryContext context)
        {
            return identifier =>
            {
                string name = identifier.ToHex();
                string[] files = Directory.GetFiles(context.Location, $"{name}-*");

                if (files.Length != 1)
                    return null;

                return BlobFactory.Create(with =>
                {
                    with.Identifier = identifier;
                    with.Path = files[0];
                });
            };
        }

        public static Func<byte[], BlobCreation> New(BlobRepositoryFactoryContext context)
        {
            return data =>
            {
                Guid identifier = Guid.NewGuid();
                string name = identifier.ToHex();

                string hash = data.ToHash();
                string path = Path.Combine(context.Location, $"{name}-{hash}");

                if (IsValidZipContent(data))
                {
                    File.WriteAllBytes(path, data);

                    return new BlobCreation
                    {
                        Success = true,
                        GetBlob = () => BlobFactory.Create(with =>
                        {
                            with.Identifier = identifier;
                            with.Path = path;
                        })
                    };
                }

                return new BlobCreation
                {
                    Success = false
                };
            };
        }

        private static bool IsValidZipContent(byte[] data)
        {
            byte[] buffer = new byte[1024];

            try
            {
                using (MemoryStream stream = new MemoryStream(data))
                using (ZipArchive archive = new ZipArchive(stream))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        using (Stream ignore = entry.Open())
                        {
                            while (ignore.Read(buffer, 0, buffer.Length) > 0) ;
                        }
                    }
                }

                return true;
            }
            catch (InvalidDataException)
            {
                return false;
            }
        }
    }
}