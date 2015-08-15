using System;
using System.Collections.Immutable;

namespace HarshPoint.Tests
{
    internal abstract class Universe
    {
        public String Name { get; set; }

        public static ImmutableArray<Tuple<Planet, Continent, Country, City, Street, Building>> Create()
            => ImmutableArray.Create(
                Create("Earth", "Europe", "CZ", "Prague", "Americka", "1234"),
                Create("Earth", "Europe", "CZ", "Prague", "Belgicka", "1234"),
                Create("Earth", "Europe", "CZ", "Prague", "Londynska", "1234"),
                Create("Earth", "Europe", "CZ", "Brno", "Namesti svobody", "42"),
                Create("Earth", "Europe", "CZ", "Brno", "Prazska", "1"),
                Create("Earth", "Europe", "DE", "Muenster", "Spiegelturm", "12"),
                Create("Earth", "Europe", "DE", "Muenster", "Domplatz", "12"),
                Create("Earth", "North America", "USA", "Seattle", "Microsoft Way", "1"),
                Create("Earth", "North America", "USA", "Cupertino", "Infinte Loop", "1")
            );

        public static Tuple<Planet, Continent, Country, City, Street, Building> Create(
            String planet,
            String continent,
            String country,
            String city,
            String street,
            String building
        )
            => Tuple.Create(
                Create<Planet>(planet),
                Create<Continent>(continent),
                Create<Country>(country),
                Create<City>(city),
                Create<Street>(street),
                Create<Building>(building)
            );

        public static T Create<T>(String name)
            where T : Universe, new()
            => new T() { Name = name };

    }

    internal sealed class Planet : Universe { }
    internal sealed class Continent : Universe { }
    internal sealed class Country : Universe { }
    internal sealed class City : Universe { }
    internal sealed class Street : Universe { }
    internal sealed class Building : Universe { }
}
