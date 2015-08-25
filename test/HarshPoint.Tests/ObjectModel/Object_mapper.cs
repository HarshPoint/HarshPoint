using HarshPoint.Tests;
using System;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.ObjectModel
{
    public class Object_mapper : SeriloggedTest
    {
        public Object_mapper(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void IsEmpty_by_default()
        {
            var mapper = new ObjectMapper<Source, Target>();
            Assert.True(mapper.IsEmpty);
        }

        [Fact]
        public void Maps_property_by_name()
        {
            var mapper = new ObjectMapper<Source, Target>();
            mapper.Map(x => x.Prop);

            var mapping = mapper.ToMapping();
            var entry = Assert.Single(mapping.Entries);

            var src = new Source() { Prop = "424342" };
            Assert.Equal(src.Prop, entry.SourceSelector(src));
            Assert.Equal("Prop", entry.TargetAccessor.Name);
        }

        private class Source
        {
            public String Prop { get; set; }
        }

        private class Target
        {
            public String Prop { get; set; }
        }

    }
}
