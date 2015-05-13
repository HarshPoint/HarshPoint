using HarshPoint.Entity;
using System;
using Xunit;

namespace HarshPoint.Tests.Entity
{
    public class FieldAttributeTests
    {
        [Theory]
        [InlineData("224e003b2c494aeb81c8bebae450ceff")]
        [InlineData("77b46cb9-7cf0-4dfe-9224-49f9281d719b")]
        [InlineData("{b6fc2f2f-5dd9-4f24-ad85-ece09fd02314}")]
        public void Ctor_accepts_a_valid_id(String validId)
        {
            Assert.Equal(new Guid(validId), new FieldAttribute(validId).FieldId);
        }

        [Theory]
        [InlineData(typeof(ArgumentNullException), null)]
        [InlineData(typeof(ArgumentOutOfRangeException), "")]
        [InlineData(typeof(ArgumentOutOfRangeException), "    ")]
        [InlineData(typeof(ArgumentOutOfRangeException), "  not a valid hexadecimal string   ")]
        [InlineData(typeof(ArgumentOutOfRangeException), "4e003b2c494aeb81c8bebae450ceff")]
        public void Ctor_rejects_an_invalid_id(Type exceptionType, String invalidId)
        {
            Assert.Throws(exceptionType, () => new FieldAttribute(invalidId));
        }
    }
}
