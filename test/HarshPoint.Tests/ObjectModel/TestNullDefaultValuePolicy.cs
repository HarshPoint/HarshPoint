using HarshPoint.ObjectModel;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.ObjectModel
{
    public class TestNullDefaultValuePolicy : SeriloggedTest
    {
        public TestNullDefaultValuePolicy(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [InlineData(typeof(Object))]
        [InlineData(typeof(IEnumerable<String>))]
        [InlineData(typeof(Decimal?))]
        public void Nullable_types_are_supported(Type type)
        {
            var policy = new NullDefaultValuePolicy();
            Assert.True(policy.SupportsType(type.GetTypeInfo()));
        }


        [Theory]
        [InlineData(typeof(Int32))]
        [InlineData(typeof(Decimal))]
        [InlineData(typeof(DayOfWeek))]
        [InlineData(typeof(DateTimeOffset))]
        public void Value_types_are_not_supported(Type type)
        {
            var policy = new NullDefaultValuePolicy();
            Assert.False(policy.SupportsType(type.GetTypeInfo()));
        }

        [Fact]
        public void Returns_true_for_Null_valuess()
        {
            var policy = new NullDefaultValuePolicy();
            Assert.True(policy.IsDefaultValue(null));
        }


        [Fact]
        public void Returns_true_for_NullableInt32()
        {
            var policy = new NullDefaultValuePolicy();
            Assert.True(policy.IsDefaultValue(new Int32?()));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" \t  \n  ")]
        [InlineData(0)]
        [InlineData(false)]
        [InlineData(new Int32[0])]
        public void Returns_false_for_non_null_values(Object value)
        {
            var policy = new NullDefaultValuePolicy();
            Assert.False(policy.IsDefaultValue(value));
        }
    }
}
