using System;
using Xunit;

namespace HarshPoint.Tests
{
    public class ExpressionExtensionsTests
    {
        public String TestProperty
        {
            get;
            set;
        }

        [Fact]
        public void GetMemberName_finds_captured_member_access()
        {
            Assert.Equal("TestProperty", ExpressionExtensions.GetMemberName(() => TestProperty));
        }

        [Fact]
        public void GetMemberName_finds_argument_member_access()
        {
            Assert.Equal("TestProperty", ExpressionExtensions.GetMemberName((ExpressionExtensionsTests arg) => arg.TestProperty));
        }

        [Fact]
         public void GetMemberName_finds_nested_expression()
        {
            Assert.Equal("TestProperty.Length", ExpressionExtensions.GetMemberName((ExpressionExtensionsTests arg) => arg.TestProperty.Length));
        }
    }
}
