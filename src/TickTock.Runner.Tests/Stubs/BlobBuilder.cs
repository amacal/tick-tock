using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace TickTock.Runner.Tests.Stubs
{
    public class BlobBuilder
    {
        private readonly Dictionary<string, byte[]> files;

        public BlobBuilder()
        {
            files = new Dictionary<string, byte[]>();
        }

        public void File(string path)
        {
            files[path] = new byte[0];
        }

        public byte[] Build()
        {
            using (MemoryStream memory = new MemoryStream())
            {
                using (ZipArchive archive = new ZipArchive(memory, ZipArchiveMode.Create, true))
                {
                    foreach (string file in files.Keys)
                    {
                        archive.CreateEntry(file, CompressionLevel.Fastest);
                    }
                }

                return memory.ToArray();
            }
        }
    }
}