using HarshPoint.Provisioning.Implementation;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning.Implementation
{
    public class NestedResolveResults : SeriloggedTest
    {
        public NestedResolveResults(ITestOutputHelper output) : base(output)
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

        [Fact]
        public void Extract_all_components()
        {
            var vinaDelMar = CreateVinaDelMar();

            var components = vinaDelMar.ExtractComponents(
                typeof(Planet),
                typeof(Continent),
                typeof(Country),
                typeof(City)
            );

            Assert.NotNull(components);
            Assert.All(components, Assert.NotNull);

            Assert.Equal(
                new[] { "Earth", "South America", "Chile", "Viña del Mar" },
                components.Cast<Universe>().Select(c => c.Name)
            );
        }

        [Fact]
        public void Extract_some_components()
        {
            var vinaDelMar = CreateVinaDelMar();

            var components = vinaDelMar.ExtractComponents(
                typeof(Continent),
                typeof(City)
            );

            Assert.NotNull(components);
            Assert.All(components, Assert.NotNull);

            Assert.Equal(
                new[] { "South America", "Viña del Mar" },
                components.Cast<Universe>().Select(c => c.Name)
            );
        }

        [Fact]
        public void Fails_to_extract_without_value()
        {
            var vinaDelMar = CreateVinaDelMar();

            Assert.Throws<ArgumentOutOfRangeException>(
                () => vinaDelMar.ExtractComponents(typeof(Continent))
            );
        }

        private static NestedResolveResult<City> CreateVinaDelMar()
            => NestedResolveResult.Pack(
                Universe.Create<City>("Viña del Mar"),
                NestedResolveResult.Pack(
                    Universe.Create<Country>("Chile"),
                    NestedResolveResult.Pack(
                        Universe.Create<Continent>("South America"),
                        Universe.Create<Planet>("Earth")
                    )
                )
            );
    }
}
