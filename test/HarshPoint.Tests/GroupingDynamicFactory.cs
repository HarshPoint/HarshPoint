using System;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests
{
    public class GroupingDynamicFactory : SeriloggedTest
    {
        public GroupingDynamicFactory(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Creates_a_grouping()
        {
            var factory = new HarshGroupingDynamicFactory(typeof(Int32), typeof(String));
            var grouping = factory.Create(42, new[] { "a", "b", "c" });

            var typed = Assert.IsType<HarshGrouping<Int32, String>>(grouping);
            Assert.Equal(42, typed.Key);
            Assert.Equal("a b c".Split(' '), typed);
        }
    }
}
