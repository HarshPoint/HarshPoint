using HarshPoint.ObjectModel;
using System;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning
{
    public class ParameterSetResolving : SeriloggedTest
    {
        public ParameterSetResolving(ITestOutputHelper output)
            : base(output)
        {
        }

        [Fact]
        public void Single_parameter_set_no_params_is_resolved()
        {
            var set = Resolve(new ImplicitEmptyParamSet());
            Assert.NotNull(set);
            Assert.True(set.IsDefault);
            Assert.True(set.IsImplicit);
            Assert.Empty(set.Parameters);
        }

        [Fact]
        public void Single_parameter_set_with_params_is_resolved()
        {
            var set = Resolve(new ImplicitTwoParamSet()
            {
                Param1 = "123",
                Param2 = "234",
            });
            Assert.NotNull(set);
            Assert.True(set.IsDefault);
            Assert.True(set.IsImplicit);
            Assert.Equal(2, set.Parameters.Count);
        }

        [Fact]
        public void Set1_gets_resolved_when_param_not_null()
        {
            var set = Resolve(new TwoSets() { Param1 = "42" });
            Assert.NotNull(set);
            Assert.Equal("Set1", set.Name);
            Assert.False(set.IsDefault);
        }


        [Fact]
        public void Set2_gets_resolved_when_param_not_null()
        {
            var set = Resolve(new TwoSets() { Param2 = "42" });
            Assert.NotNull(set);
            Assert.Equal("Set2", set.Name);
            Assert.True(set.IsDefault);
        }


        [Fact]
        public void Set2_gets_resolved_when_no_params_are_set()
        {
            var set = Resolve(new TwoSets());
            Assert.NotNull(set);
            Assert.Equal("Set2", set.Name);
            Assert.True(set.IsDefault);
        }

        [Fact]
        public void Fails_when_parameters_from_two_sets_are_selected()
        {
            Assert.Throws<InvalidOperationException>(
                () => Resolve(new TwoSets() { Param1 = "1", Param2 = "2" })
            );
        }

        private ParameterSet Resolve<T>(T target)
        {
            var builder = new ParameterSetBuilder(typeof(T));
            var resolver = new ParameterSetResolver(target, builder.Build());
            return resolver.Resolve();
        }

        private sealed class ImplicitEmptyParamSet
        {
        }

        private sealed class ImplicitTwoParamSet
        {
            [Parameter]
            public String Param1 { get; set; }

            [Parameter]
            public String Param2 { get; set; }
        }

        [DefaultParameterSet("Set2")]
        private sealed class TwoSets
        {
            [Parameter(ParameterSetName = "Set1")]
            public String Param1 { get; set; }

            [Parameter(ParameterSetName = "Set2")]
            public String Param2 { get; set; }
        }
    }
}
