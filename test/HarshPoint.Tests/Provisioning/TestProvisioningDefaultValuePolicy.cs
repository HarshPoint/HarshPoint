using HarshPoint.Provisioning.Implementation;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning
{
    public class TestProvisioningDefaultValuePolicy : SeriloggedTest
    {
        public TestProvisioningDefaultValuePolicy(ITestOutputHelper output) : base(output)
        {
        }



        [Theory]
        [InlineData(typeof(Object))]
        [InlineData(typeof(IEnumerable<String>))]
        [InlineData(typeof(Decimal?))]
        [InlineData(typeof(Guid))]
        public void Expected_types_are_supported(Type type)
        {
            var policy = ProvisioningDefaultValuePolicy.Instance;
            Assert.True(policy.SupportsType(type.GetTypeInfo()));
        }


        [Theory]
        [InlineData(typeof(Int32))]
        [InlineData(typeof(Decimal))]
        [InlineData(typeof(DayOfWeek))]
        [InlineData(typeof(DateTimeOffset))]
        public void Value_types_are_not_supported(Type type)
        {
            var policy = ProvisioningDefaultValuePolicy.Instance;
            Assert.False(policy.SupportsType(type.GetTypeInfo()));
        }

        [Fact]
        public void Returns_true_for_Null_values()
        {
            var policy = ProvisioningDefaultValuePolicy.Instance;
            Assert.True(policy.IsDefaultValue(null));
        }


        [Fact]
        public void Returns_true_for_NullableInt32()
        {
            var policy = ProvisioningDefaultValuePolicy.Instance;
            Assert.True(policy.IsDefaultValue(new Int32?()));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(false)]
        public void Returns_false_for_expected_values(Object value)
        {
            var policy = ProvisioningDefaultValuePolicy.Instance;
            Assert.False(policy.IsDefaultValue(value));
        }

        [Fact]
        public void Returns_false_for_new_guid()
        {
            var policy = ProvisioningDefaultValuePolicy.Instance;
            Assert.False(policy.IsDefaultValue(Guid.NewGuid()));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" \t  \n  ")]
        [InlineData(new Int32[0])]
        public void Returns_true_for_expected_values(Object value)
        {
            var policy = ProvisioningDefaultValuePolicy.Instance;
            Assert.True(policy.IsDefaultValue(value));
        }

        [Fact]
        public void Returns_true_for_empty_guid()
        {
            var policy = ProvisioningDefaultValuePolicy.Instance;
            Assert.True(policy.IsDefaultValue(Guid.Empty));
        }
    }
}
