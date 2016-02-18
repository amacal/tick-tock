using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TickTock.Core.Blobs
{
    public class BlobRepository
    {
        private readonly string location;

        public BlobRepository(string location)
        {
            this.location = location;
        }

        public Blob GetById(Guid identifier)
        {
            string name = GuidToString(identifier);
            string path = Path.Combine(location, name);

            FileInfo file = new FileInfo(path);
            MD5 md5 = new MD5Cng();

            return new Blob
            {
                Identifier = identifier,
                Hash = BytesToString(md5.ComputeHash(file.OpenRead())),
                Size = file.Length
            };
        }

        public byte[] GetData(Guid id)
        {
            return null;
        }

        public Guid Add(byte[] data)
        {
            Guid identifier = Guid.NewGuid();
            string name = GuidToString(identifier);

            string path = Path.Combine(location, name);
            File.WriteAllBytes(path, data);

            return identifier;
        }

        private static string GuidToString(Guid value)
        {
            return BytesToString(value.ToByteArray());
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