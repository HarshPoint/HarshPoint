using HarshPoint.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;

namespace HarshPoint.Tests.Entity
{
    public class ContentTypeAttributeTests
    {
        [Theory]
        [InlineData("42")]
        [InlineData("be")]
        [InlineData("EF")]
        [InlineData("ab801cf740b44f41b4207c839ec3966d")]
        [InlineData("49EAB4FCFC344A2C983A13A262462D4D")]
        public void Ctor_accepts_a_valid_relative_id(String validRelativeId)
        {
            new ContentTypeAttribute(validRelativeId);
        }

        [Theory]
        [InlineData(typeof(ArgumentNullException), null)]
        [InlineData(typeof(ArgumentOutOfRangeException), "")]
        [InlineData(typeof(ArgumentOutOfRangeException), "  \t  ")]
        [InlineData(typeof(ArgumentOutOfRangeException), "12345")]
        [InlineData(typeof(ArgumentOutOfRangeException), "zz")]
        [InlineData(typeof(ArgumentOutOfRangeException), "00")]
        [InlineData(typeof(ArgumentOutOfRangeException), "ab801cf740b44f41b4207c839ec3966d00")]
        public void Ctor_rejects_an_invalid_relative_id(Type exceptionType, String invalidRelativeId)
        {
            Assert.Throws(exceptionType, () => new ContentTypeAttribute(invalidRelativeId));
        }

    }
}
