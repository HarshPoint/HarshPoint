using HarshPoint.ObjectModel;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning
{
    public class ParameterSetMetadataBuilding : SeriloggedTest
    {
        public ParameterSetMetadataBuilding(ITestOutputHelper output)
            : base(output)
        {
        }

        [Fact]
        public void Parameters_get_metadata()
        {
            var sets = Build<OnlyDefaultParameterSet>();
            var set = Assert.Single(sets);

            Assert.Equal(ParameterSet.ImplicitParameterSetName, set.Name);
            Assert.True(set.IsDefault);
            Assert.Equal(2, set.Parameters.Count);

            var names = set.Parameters.Select(p => p.Name).ToArray();

            Assert.Contains("Param1", names);
            Assert.Contains("Param2", names);
            Assert.DoesNotContain("NotAParam", names);
        }

        [Fact]
        public void No_parameters_get_empty_set()
        {
            var sets = Build<NoParameters>();
            var set = Assert.Single(sets);

            Assert.Equal("__DefaultParameterSet", set.Name);
            Assert.True(set.IsDefault);
            Assert.Empty(set.Parameters);
        }

        [Fact]
        public void Two_sets_implicit_default()
        {
            var sets = Build<TwoSetsImplicitDefault>();

            Assert.Equal(2, sets.Length);

            Assert.Equal("Set1", sets[0].Name);
            Assert.Equal("Set2", sets[1].Name);

            Assert.True(sets[0].IsDefault);
            Assert.False(sets[1].IsDefault);
        }

        [Fact]
        public void Two_sets_explicit_default()
        {
            var sets = Build<TwoSetsExplicitDefault>();
            Assert.Equal(2, sets.Length);

            Assert.Equal("Set1", sets[0].Name);
            Assert.Equal("Set2", sets[1].Name);

            Assert.False(sets[0].IsDefault);
            Assert.True(sets[1].IsDefault);
        }

        [Fact]
        public void Parameter_in_multiple_sets()
        {
            var sets = Build<ParameterInMultipleSets>();
            Assert.Equal(3, sets.Length);

            Assert.Equal("Set1", sets[0].Name);
            Assert.Equal("Set2", sets[1].Name);
            Assert.Equal("Set3", sets[2].Name);

            Assert.False(sets[0].IsDefault);
            Assert.False(sets[1].IsDefault);
            Assert.True(sets[2].IsDefault);

            Assert.Equal(2, sets[0].Parameters.Count);
            Assert.Equal(2, sets[1].Parameters.Count);
            Assert.Equal(2, sets[2].Parameters.Count);

            Assert.Equal("ParamInSet1and2", sets[0].Parameters[0].Name);
            Assert.Equal("CommonParam", sets[0].Parameters[1].Name);

            Assert.Equal("ParamInSet1and2", sets[1].Parameters[0].Name);
            Assert.Equal("CommonParam", sets[1].Parameters[1].Name);

            Assert.Equal("ParamInSet3", sets[2].Parameters[0].Name);
            Assert.Equal("CommonParam", sets[2].Parameters[1].Name);
        }

        [Fact]
        public void Implicit_named_set_is_default()
        {
            var sets = Build<ImplicitNamedSetIsDefault>();
            var set = Assert.Single(sets);

            Assert.Equal("Set1", set.Name);
            Assert.True(set.IsDefault);
            Assert.Equal(2, set.Parameters.Count);
        }

        [Fact]
        public void Fails_internal_writable_param()
        {
            Assert.Throws<HarshObjectMetadataException>(
                   () => Build<InternalWritableParam>()
               );
        }

        [Fact]
        public void Fails_when_default_set_doesnt_exist()
        {
            Assert.Throws<HarshObjectMetadataException>(
                () => Build<WithNonexistentDefaultParamSet>()
            );
        }

        [Fact]
        public void Fails_Parameter_both_common_and_in_set()
        {
            Assert.Throws<HarshObjectMetadataException>(
                () => Build<ParameterBothCommonAndInSet>()
            );
        }

        [Fact]
        public void Fails_Parameter_twice_in_one_set()
        {
            Assert.Throws<HarshObjectMetadataException>(
                () => Build<ParameterInOneSetTwice>()
            );
        }

        [Fact]
        public void Fails_non_readable_param()
        {
            Assert.Throws<HarshObjectMetadataException>(
                () => Build<NonReadableParam>()
            );
        }

        [Fact]
        public void Fails_nonnullable_mandatory()
        {
            var exc = Assert.Throws<HarshObjectMetadataException>(
                () => Build<NonnullableMandatoryParamType>()
            );
        }

        private static ParameterSet[] Build<T>()
        {
            var builder = new ParameterSetBuilder(typeof(T));
            return builder.Build().OrderBy(s => s.Name).ToArray();
        }

        private sealed class OnlyDefaultParameterSet
        {
            [Parameter]
            public String Param1 { get; set; }

            [Parameter]
            public String Param2 { get; set; }

            public String NotAParam { get; set; }
        }

        private sealed class NoParameters
        {
            public Int32 NotAParam { get; set; }
        }

        private sealed class TwoSetsImplicitDefault
        {
            [Parameter(ParameterSetName = "Set1")]
            public String ParamInSet1 { get; set; }

            [Parameter(ParameterSetName = "Set2")]
            public String ParamInSet2 { get; set; }

            [Parameter]
            public Boolean CommonParam { get; set; }

            public Object NotAParam { get; set; }
        }

        [DefaultParameterSet("Set2")]
        private sealed class TwoSetsExplicitDefault
        {
            [Parameter(ParameterSetName = "Set1")]
            public String ParamInSet1 { get; set; }

            [Parameter(ParameterSetName = "Set2")]
            public String ParamInSet2 { get; set; }

            [Parameter]
            public Boolean CommonParam { get; set; }

            public Object NotAParam { get; set; }
        }

        private sealed class ImplicitNamedSetIsDefault
        {
            [Parameter(ParameterSetName = "Set1")]
            public String ParamInSet1 { get; set; }

            [Parameter]
            public String CommonParam { get; set; }
        }

        [DefaultParameterSet("Set3")]
        private sealed class ParameterInMultipleSets
        {
            [Parameter(ParameterSetName = "Set1")]
            [Parameter(ParameterSetName = "Set2")]
            public String ParamInSet1and2 { get; set; }

            [Parameter(ParameterSetName = "Set3")]
            public String ParamInSet3 { get; set; }

            [Parameter]
            public String CommonParam { get; set; }
        }

        private sealed class ParameterBothCommonAndInSet
        {
            [Parameter]
            [Parameter(ParameterSetName = "Set1")]
            public String BadParam { get; set; }
        }

        private sealed class ParameterInOneSetTwice
        {
            [Parameter(ParameterSetName = "Set1")]
            [Parameter(ParameterSetName = "Set1")]
            public String BadParam { get; set; }
        }

        private sealed class NonReadableParam
        {
            [Parameter]
            public String NonReadable
            {
                set { }
            }
        }

        private sealed class InternalWritableParam
        {
            [Parameter]
            public String InternalWritable
            {
                get;
                internal set;
            }
        }

        [DefaultParameterSet("TestParamSet")]
        private sealed class WithNonexistentDefaultParamSet
        {
        }

        private sealed class NonnullableMandatoryParamType
        {
            [Parameter(Mandatory = true)]
            public Int32 NotNullableMandatory { get; set; }
        }
    }
}
