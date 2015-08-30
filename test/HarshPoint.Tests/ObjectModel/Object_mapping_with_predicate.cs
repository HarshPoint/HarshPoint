using HarshPoint.Tests;
using System;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.ObjectModel
{
    public class Object_mapping_with_predicate : SeriloggedTest
    {
        private readonly ObjectMapping _map;

        public Object_mapping_with_predicate(ITestOutputHelper output) : base(output)
        {
            var mapper = new ObjectMapper<Source, Target>();

            mapper
                .Map(x => x.TargetProp)
                .From(x => x.SourceProp)
                .When(x => x.SourceProp.Length > 1);

            _map = mapper.ToMapping();
        }

        [Fact]
        public void Doesnt_set_when_predicate_false()
        {
            var source = new Source() { SourceProp = "a" };
            var target = new Target() { TargetProp = null };

            Assert.Empty(_map.Apply(source, target));
            Assert.Null(target.TargetProp);
        }

        [Fact]
        public void Changes_when_predicate_true()
        {
            var source = new Source() { SourceProp = "aa" };
            var target = new Target() { TargetProp = null };

            var a =Assert.Single(_map.Apply(source, target));
            Assert.Equal("aa", a.SourceValue);
            Assert.Null(a.TargetValue);
            Assert.False(a.ValuesEqual);
            Assert.Equal("aa", target.TargetProp);
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
