using HarshPoint.Tests;
using System;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.ObjectModel
{
    public class Object_mapping : SeriloggedTest
    {
        private readonly ObjectMapping _map;

        public Object_mapping(ITestOutputHelper output) : base(output)
        {
            var builder = new ObjectMapper<Source, Target>();

            builder
                .Map(x => x.TargetProp)
                .From(x => x.SourceProp);

            _map = builder.ToMapping();
        }

        [Fact]
        public void Gets_target_properties()
        {
            var expr = Assert.Single(
                _map.GetTargetExpressions<Target>()
            );

            var prop = expr.ExtractLastPropertyAccess();
            Assert.Equal("TargetProp", prop.Name);
        }

        [Fact]
        public void Changes_null_to_string()
        {
            var source = new Source() { SourceProp = "42" };
            var target = new Target() { TargetProp = null };

            Assert.True(_map.Apply(source, target));
            Assert.Equal("42", target.TargetProp);
        }

        [Fact]
        public void Doesnt_change_null_to_null()
        {
            var source = new Source() { SourceProp = null };
            var target = new Target() { TargetProp = null };

            Assert.False(_map.Apply(source, target));
        }

        [Fact]
        public void Doesnt_change_string_to_string()
        {
            var source = new Source() { SourceProp = "string" };
            var target = new Target() { TargetProp = "string" };

            Assert.False(_map.Apply(source, target));
        }

        private class Source
        {
            public String SourceProp { get; set; }
        }

        private class Target
        {
            public String TargetProp { get; set; }
        }
    }
}
