using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
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

            Assert.Equal("__DefaultParameterSet", set.Name);
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
        public void Two_sets_implicit_default_one_common()
        {
            var sets = Build<TwoSetsImplicitDefault>();
            Assert.Equal(2, sets.Length);

            Assert.Equal("Set1", sets[0].Name);
            Assert.Equal("Set2", sets[1].Name);

            Assert.True(sets[0].IsDefault);
            Assert.False(sets[1].IsDefault);

            Assert.Equal(2, sets[0].Parameters.Count);
            Assert.Equal(2, sets[1].Parameters.Count);

            Assert.Equal("ParamInSet1", sets[0].Parameters[0].Name);
            Assert.Equal("CommonParam", sets[0].Parameters[1].Name);

            Assert.Equal("ParamInSet2", sets[1].Parameters[0].Name);
            Assert.Equal("CommonParam", sets[1].Parameters[1].Name);
        }

        [Fact]
        public void Two_sets_explicit_default()
        {
            var sets = Build<TwoSetsImplicitDefault>("Set2");
            Assert.Equal(2, sets.Length);

            Assert.Equal("Set1", sets[0].Name);
            Assert.Equal("Set2", sets[1].Name);

            Assert.False(sets[0].IsDefault);
            Assert.True(sets[1].IsDefault);
        }

        [Fact]
        public void Parameter_in_multiple_sets()
        {
            var sets = Build<ParameterInMultipleSets>("Set3");
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
        public void Fails_Parameter_both_common_and_in_set()
        {
            Assert.Throws<HarshProvisionerMetadataException>(
                () => Build<ParameterBothCommonAndInSet>()
            );
        }

        [Fact]
        public void Fails_Parameter_twice_in_one_set()
        {
            Assert.Throws<HarshProvisionerMetadataException>(
                () => Build<ParameterInOneSetTwice>()
            );
        }

        [Fact]
        public void Fails_non_readable_param()
        {
            Assert.Throws<HarshProvisionerMetadataException>(
                () => Build<NonReadableParam>()
            );
        }

        [Fact]
        public void Fails_internal_writable_param()
        {
            Assert.Throws<HarshProvisionerMetadataException>(
                () => Build<InternalWritableParam>()
            );
        }

        private static ParameterSetMetadata[] Build<T>(String defaultParameterSetName = null)
        {
            var builder = new ParameterSetMetadataBuilder(typeof(T));

            if (defaultParameterSetName != null)
            {
                builder.DefaultParameterSetName = defaultParameterSetName;
            }

            return builder.Build().ToArray();
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
                get { return null; }
                internal set { }
            }
        }
    }
}
