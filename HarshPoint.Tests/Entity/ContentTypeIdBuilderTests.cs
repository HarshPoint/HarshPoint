using HarshPoint.Entity;
using HarshPoint.Entity.Metadata;
using System;
using System.Reflection;
using Xunit;

namespace HarshPoint.Tests.Entity
{
    public class ContentTypeIdBuilderTests
    {
        public class Negative
        {
            [ContentType(null)]
            public class NullCtid : HarshEntity { }

            [ContentType("")]
            public class EmptyStringCtid : HarshEntity { }

            [ContentType("zzzz")]
            public class ZzzzCtid : HarshEntity { }

            [ContentType("01")]
            public class RelativeCtid : HarshEntity { }

            [ContentType("01")]
            public class AnotherRelativeCtid : HarshEntity { }

            [ContentType("00")]
            public class ZeroRelativeCtid : HarshEntity { }

            public class NotACt : HarshEntity { }

            public class ChildOfNotACt : NotACt { }

            [Theory]
            [InlineData(typeof(NullCtid), typeof(ArgumentNullException))]
            [InlineData(typeof(EmptyStringCtid), typeof(ArgumentOutOfRangeException))]
            [InlineData(typeof(ZzzzCtid), typeof(ArgumentOutOfRangeException))]
            [InlineData(typeof(AnotherRelativeCtid), typeof(InvalidOperationException))]
            [InlineData(typeof(ZeroRelativeCtid), typeof(ArgumentOutOfRangeException))]
            [InlineData(typeof(NotACt), typeof(InvalidOperationException))]
            [InlineData(typeof(ChildOfNotACt), typeof(InvalidOperationException))]
            public void Throws_for_an_invalid_ContentType(Type ct, Type exceptionType)
            {
                Assert.Throws(exceptionType, () =>
                {
                    new ContentTypeIdBuilder(ct.GetTypeInfo()).ToString();
                });
            }
        }

        public class Positive
        {
            [Theory]
            [InlineData("0x01", typeof(AbsoluteCtid))]
            [InlineData("0x0102", typeof(AbsoluteCtidSubid))]
            [InlineData("0x01006ABB016532AA4626A4B3A4B1022A0524", typeof(AbsoluteCtidGuidSubid))]
            [InlineData("0x0101", typeof(ChildOfAbsoluteCtid))]
            [InlineData("0x01006ABB016532AA4626A4B3A4B1022A0524", typeof(GuidChildOfAbsoluteCtid))]
            public void Builds_correct_id(String expected, Type type)
            {
                Assert.Equal(expected, new ContentTypeIdBuilder(type.GetTypeInfo()).ToString());
            }

            [ContentType("0x01")]
            public class AbsoluteCtid : HarshEntity { }

            [ContentType("0x0102")]
            public class AbsoluteCtidSubid : HarshEntity { }

            [ContentType("0x01006abb016532aa4626a4b3a4b1022a0524")]
            public class AbsoluteCtidGuidSubid : HarshEntity { }

            [ContentType("01")]
            public class ChildOfAbsoluteCtid : AbsoluteCtid { }

            [ContentType("6abb016532aa4626a4b3a4b1022a0524")]
            public class GuidChildOfAbsoluteCtid : AbsoluteCtid { }
        }
    }
}
