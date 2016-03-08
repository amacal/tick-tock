using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using TickTock.Core.Extensions;

namespace TickTock.Core.Blobs
{
    public static class BlobFactory
    {
        public static Blob Create(Action<BlobFactoryContext> with)
        {
            return with.Apply(context =>
            {
                return new Blob
                {
                    Identifier = context.Identifier,
                    GetHash = GetHash(context),
                    GetSize = GetSize(context),
                    GetFiles = GetFiles(context),
                    DeployTo = DeployTo(context)
                };
            });
        }

        private static Func<string> GetHash(BlobFactoryContext context)
        {
            return () =>
            {
                return Path.GetFileName(context.Path).Split('-')[1];
            };
        }

        private static Func<long> GetSize(BlobFactoryContext context)
        {
            return () =>
            {
                return new FileInfo(context.Path).Length;
            };
        }

        private static Func<BlobFileCollection> GetFiles(BlobFactoryContext context)
        {
            return () =>
            {
                List<BlobFile> files = new List<BlobFile>();

                using (ZipArchive archive = ZipFile.OpenRead(context.Path))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        files.Add(new BlobFile
                        {
                            Name = entry.Name,
                            Path = entry.FullName
                        });
                    }
                }

                return new BlobFileCollection
                {
                    Items = files.ToArray()
                };
            };
        }

        private static Func<string, BlobDeployment> DeployTo(BlobFactoryContext context)
        {
            return destination =>
            {
                ZipFile.ExtractToDirectory(context.Path, destination);
                List<BlobFile> files = new List<BlobFile>();

                foreach (string path in Directory.GetFiles(destination))
                {
                    files.Add(new BlobFile
                    {
                        Path = path,
                        Name = Path.GetFileName(path)
                    });
                }

                return new BlobDeployment
                {
                    Path = destination,
                    Files = new BlobFileCollection
                    {
                        Items = files.ToArray()
                    },
                    Find = Find(files)
                };
            };
        }

        private static Func<string, string> Find(List<BlobFile> files)
        {
            return name =>
            {
                return files.Where(x => x.Path.EndsWith(name, StringComparison.OrdinalIgnoreCase)).Select(x => x.Path).Single();
            };
        }
    }
}