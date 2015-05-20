using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace HarshPoint.Tests.Extensions
{
    public class TypeInfoExtensionsTests
    {
        [Theory]
        [InlineData(typeof(String), "String")]
        [InlineData(typeof(IEnumerable<>), "IEnumerable<T>")]
        [InlineData(typeof(IEnumerable<String>), "IEnumerable<String>")]
        public void GetCSharpSimpleName_returns_expected_values(Type type, String expected)
        {
            Assert.Equal(expected, type.GetTypeInfo().GetCSharpSimpleName());
        }
    }
}
