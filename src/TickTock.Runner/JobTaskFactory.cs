using System.IO;
using System.IO.Compression;
using TickTock.Core.Blobs;
using TickTock.Core.Extensions;
using TickTock.Core.Jobs;

namespace TickTock.Runner
{
    public class JobTaskFactory
    {
        private readonly string location;
        private readonly IBlobRepository repository;

        public JobTaskFactory(string location, IBlobRepository repository)
        {
            this.location = location;
            this.repository = repository;
        }

        public JobTask Create(Job job)
        {
            string name = job.Header.Identifier.ToHex();
            byte[] blob = repository.GetData(job.Data.Blob);

            string root = Path.Combine(location, name);
            string path = Path.Combine(root, "blob");

            Directory.CreateDirectory(path);

            using (MemoryStream memory = new MemoryStream(blob))
            using (ZipArchive archive = new ZipArchive(memory, ZipArchiveMode.Read))
            {
                archive.ExtractToDirectory(path);
            }

            return new JobTask
            {
                Path = root,
                Executable = Path.Combine(path, job.Data.Executable),
                Arguments = job.Data.Arguments,
                WorkingDirectory = path
            };
        }
    }
}