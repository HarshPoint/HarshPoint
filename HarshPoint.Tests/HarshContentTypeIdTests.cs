using System;
using Xunit;

namespace HarshPoint.Tests
{
    public class HarshContentTypeIdTests
    {
        [Theory]
        [InlineData(null, typeof(ArgumentNullException))]
        [InlineData("", typeof(ArgumentOutOfRangeException))]
        [InlineData("zz", typeof(ArgumentOutOfRangeException))]
        [InlineData("0xzz", typeof(ArgumentOutOfRangeException))]
        [InlineData("aaa", typeof(ArgumentOutOfRangeException))]
        [InlineData("0xaaa", typeof(ArgumentOutOfRangeException))]
        [InlineData("00", typeof(ArgumentOutOfRangeException))]
        [InlineData("001234", typeof(ArgumentOutOfRangeException))]
        [InlineData("123456789abcdef", typeof(ArgumentOutOfRangeException))]
        [InlineData("0x00", typeof(ArgumentOutOfRangeException))]
        [InlineData("0x0300", typeof(ArgumentOutOfRangeException))]
        [InlineData("0x0000", typeof(ArgumentOutOfRangeException))]
        [InlineData("0x000123456789abcde", typeof(ArgumentOutOfRangeException))]
        public void Throws_for_invalid_ids(String invalidId, Type exceptionType)
        {
            Assert.Throws(exceptionType, () => HarshContentTypeId.Parse(invalidId));
        }

        [Theory]
        [InlineData("01", "01")]
        [InlineData("aF", "AF")]
        [InlineData("00112233445566778899AaBbcCdDeEfF", "00112233445566778899AABBCCDDEEFF")]
        public void Parses_relative_ids(String input, String expectedToString)
        {
            var result = HarshContentTypeId.Parse(input);
            Assert.False(result.IsAbsolute);
            Assert.Equal(expectedToString, result.ToString());
        }

        [Theory]
        [InlineData("0x", "0x")]
        [InlineData("0x04", "0x04")]
        [InlineData("0x0401ab", "0x0401AB")]
        [InlineData("0x0011223344556677889900aabbccddeeff", "0x0011223344556677889900AABBCCDDEEFF")]
        [InlineData("0x040011223344556677889900aabbccddeeff", "0x040011223344556677889900AABBCCDDEEFF")]
        public void Parses_absolute_ids(String input, String expectedToString)
        {
            var result = HarshContentTypeId.Parse(input);
            Assert.True(result.IsAbsolute);
            Assert.Equal(expectedToString, result.ToString());
        }

        [Fact]
        public void Fails_to_append_null()
        {
            Assert.Throws<ArgumentNullException>(() => HarshContentTypeId.Parse("0x").Append(null));
        }

        [Fact]
        public void Fails_to_append_absolute()
        {
            var ctid1 = HarshContentTypeId.Parse("0x01");
            var ctid2 = HarshContentTypeId.Parse("0x01");

            Assert.Throws<ArgumentOutOfRangeException>(() => ctid1.Append(ctid2));
        }

        [Theory]
        [InlineData("0101", "01", "01")]
        [InlineData("010011223344556677889900AABBCCDDEEFF", "01", "11223344556677889900aabbccddeeff")]
        [InlineData("0x0101", "0x01", "01")]
        [InlineData("0xCC0011223344556677889900AABBCCDDEEFF", "0xcc", "11223344556677889900aabbccddeeff")]
        public void Appends_two_ids(String expected, String a, String b)
        {
            var ctidA = HarshContentTypeId.Parse(a);
            var ctidB = HarshContentTypeId.Parse(b);

            Assert.Equal(expected, ctidA.Append(ctidB).ToString());
        }

        [Theory]
        [InlineData(true, "0x0101", "0x01")]
        [InlineData(true, "0x0100b95aa984636a4b9996fdbbb47966bbec", "0x01")]
        [InlineData(true, "0x010100b95aa984636a4b9996fdbbb47966bbec", "0x0101")]
        [InlineData(true, "0x0100b95aa984636a4b9996fdbbb47966bbec00b95aa984636a4b9996fdbbb47966bbec", "0x01")]
        [InlineData(true, "0x0100b95aa984636a4b9996fdbbb47966bbec00b95aa984636a4b9996fdbbb47966bbec", "0x0100b95aa984636a4b9996fdbbb47966bbec")]

        [InlineData(false, "0x0101", "0x0101")]
        [InlineData(false, "0x0101", "0x02")]
        [InlineData(false, "0x0101", "0x0200b95aa984636a4b9996fdbbb47966bbec")]
        public void IsChildOf(Boolean expected, String ctidStr, String parentStr)
        {
            var ctid = HarshContentTypeId.Parse(ctidStr);
            var parent = HarshContentTypeId.Parse(parentStr);

            Assert.Equal(expected, ctid.IsChildOf(parent));
        }

        [Theory]
        [InlineData(typeof(ArgumentOutOfRangeException), "0x01", "01")]
        [InlineData(typeof(InvalidOperationException), "01", "0x01")]
        public void IsChildOf_fails(Type exceptionType, String ctidStr, String parentStr)
        {
            var ctid = HarshContentTypeId.Parse(ctidStr);
            var parent = HarshContentTypeId.Parse(parentStr);

            Assert.Throws(exceptionType, () => ctid.IsChildOf(parent));
        }

        [Theory]

        [InlineData(true, "0x0101", "0x01")]
        [InlineData(true, "0x0100b95aa984636a4b9996fdbbb47966bbec", "0x01")]
        [InlineData(true, "0x010100b95aa984636a4b9996fdbbb47966bbec", "0x0101")]

        [InlineData(false, "0x0101", "0x0101")]
        [InlineData(false, "0x0101", "0x02")]
        [InlineData(false, "0x0101", "0x0200b95aa984636a4b9996fdbbb47966bbec")]

        [InlineData(false, "0x010101", "0x01")]
        [InlineData(false, "0x0100b95aa984636a4b9996fdbbb47966bbec00b95aa984636a4b9996fdbbb47966bbec", "0x01")]
        public void IsDirectChildOf(Boolean expected, String ctidStr, String parentStr)
        {
            var ctid = HarshContentTypeId.Parse(ctidStr);
            var parent = HarshContentTypeId.Parse(parentStr);

            Assert.Equal(expected, ctid.IsDirectChildOf(parent));
        }
    }
}
