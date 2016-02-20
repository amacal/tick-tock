using System;
using System.Security.Cryptography;
using System.Text;

namespace TickTock.Core.Extensions
{
    public static class BytesExtensions
    {
        public static string ToHex(this Guid value)
        {
            return value.ToString("N");
        }

        public static string ToHash(this byte[] bytes)
        {
            using (MD5 algorithm = MD5.Create())
            {
                return ToHex(algorithm.ComputeHash(bytes));
            }
        }

        public static string ToHex(this byte[] bytes)
        {
            int length = bytes.Length * 2;
            StringBuilder builder = new StringBuilder(length);

            foreach (byte i in bytes)
                builder.AppendFormat("{0:x2}", i);

            return builder.ToString();
        }
    }
}