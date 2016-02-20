using FluentAssertions;
using System;
using TickTock.Core.Extensions;
using Xunit;

namespace TickTock.Core.Tests.Extensions
{
    public class BytesExtensionsFixture
    {
        [Fact]
        public void ConvertsGuidToHex()
        {
            Guid identifier = Guid.Parse("D566585D-55F5-4D01-ACB3-3A5CA5B2665A");

            string hex = identifier.ToHex();

            hex.Should().Be("d566585d55f54d01acb33a5ca5b2665a");
        }

        [Fact]
        public void ConvertsGuidToHexWhichCanBeParsed()
        {
            Guid identifier = Guid.Parse("D566585D-55F5-4D01-ACB3-3A5CA5B2665A");

            string hex = identifier.ToHex();
            Guid parsed = Guid.Parse(hex);

            parsed.Should().Be(identifier);
        }

        [Fact]
        public void ConvertsArrayToHex()
        {
            byte[] array = new byte[]
            {
                0xd5, 0x66, 0x58, 0x5d, 0x55, 0xf5, 0x4d, 0x01,
                0xac, 0xb3, 0x3a, 0x5c, 0xa5, 0xb2, 0x66, 0x5a
            };

            string hex = array.ToHex();

            hex.Should().Be("d566585d55f54d01acb33a5ca5b2665a");
        }
    }
}