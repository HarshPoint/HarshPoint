using HarshPoint.Provisioning.Implementation;
using System;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning.Implementation
{
    public class NestedResolveResultPacking : SeriloggedTest
    {
        public NestedResolveResultPacking(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Packs_non_nested_parent()
        {
            var packed = NestedResolveResult.Pack("42", "parent");

            Assert.Equal("42", packed.Value);

            var parent = Assert.Single(packed.Parents);
            Assert.Equal("parent", parent);
        }

        [Fact]
        public void Packs_nested_parent()
        {
            var parent = NestedResolveResult.Pack("42", "parent");
            var nested = NestedResolveResult.Pack("nested", parent);

            Assert.Equal("nested", nested.Value);
            Assert.Equal(new[] { "parent", "42" }, nested.Parents);
        }

        [Fact]
        public void Unpacks_non_nested_parent()
        {
            var expected = "parent";
            var actual = NestedResolveResult.Unpack<String>(expected);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Unpacks_nested_parent()
        {
            var packed = NestedResolveResult.Pack(
                "baby",
                NestedResolveResult.Pack("mama", "granny")
            );

            var unpacked = NestedResolveResult.Unpack<String>(packed);
            Assert.Equal("baby", unpacked);
        }
    }
}
