using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

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
            string name = GuidToString(identifier);
            string[] files = Directory.GetFiles(location, $"{name}-*");

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
            string name = GuidToString(identifier);
            string[] files = Directory.GetFiles(location, $"{name}-*");

            return File.ReadAllBytes(files[0]);
        }

        public Guid Add(byte[] data)
        {
            Guid identifier = Guid.NewGuid();
            string name = GuidToString(identifier);

            string hash = BytesToHash(data);
            string path = Path.Combine(location, $"{name}-{hash}");

            File.WriteAllBytes(path, data);
            return identifier;
        }

        private static string GuidToString(Guid value)
        {
            return BytesToString(value.ToByteArray());
        }

        private static string BytesToHash(byte[] bytes)
        {
            using (MD5 algorithm = MD5.Create())
            {
                return BytesToString(algorithm.ComputeHash(bytes));
            }
        }

        private static string BytesToString(byte[] bytes)
        {
            StringBuilder builder = new StringBuilder(bytes.Length * 2);

            foreach (byte i in bytes)
                builder.AppendFormat("{0:x2}", i);

            return builder.ToString();
        }
    }
}