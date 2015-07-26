using HarshPoint.Provisioning.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace HarshPoint.Tests.Provisioning.Implementation
{
    public class NestedResultsProofOfConcept
    {
        [Fact]
        public void Inner_grouping()
        {
            /* 
                Tuple[ Planet, Continent, Country, City, Street, Building ]
                Grouping[ Tuple[ Planet, Continent, Country, City, Street ], Building ]
                   

                Tuple[ Planet, Continent, Country, City, Grouping[Street, Building] ]
                Tuple[ Planet, Continent, Country, Tuple[ City, Grouping[Street, Building] ] ]
                Tuple[ Planet, Tuple[ Continent, Country ], Tuple[ City, Grouping[Street, Building] ] ]

                Tuple[ Planet, Grouping[ Tuple[ Continent, Country ], Tuple[ City, Grouping[Street, Building] ] ] ]

                Tuple<
                    Planet,
                    IGrouping<
                        Tuple<Continent, Country>, 
                        Tuple<
                            City,
                            IGrouping<Street, Building>
                        >
                    >
                >

                GroupBy Tuple(Item1..Item5) => Item6
                Tuple (
                    LiftOutOfKey Item1
                    Tuple( LiftOutOfKey Item2, LiftOutOfKey Item3 )
                    Tuple( LiftOutOfKey Item4, Grouping (LiftOutOfKey Item5) )
                )

                GroupBy (Tuple (Item1, Item2) => Item3)
                Tuple( LiftOutOfKey Item1, Grouping LiftOutOfKey Item2)
            */

            var source = Universe.Create();
            var step1 = source.GroupBy(
                x => Tuple.Create(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5),
                x => x.Item6,
                (k, g) => Tuple.Create(
                    k.Item1, 
                    Tuple.Create(k.Item2, k.Item3), 
                    Tuple.Create(k.Item4, Grouping(k.Item5, g))
                )
            );

            var step2 = step1.GroupBy(
                x => Tuple.Create(x.Item1, x.Item2),
                x => x.Item3,
                (k, g) => Tuple.Create(k.Item1, Grouping(k.Item2, g))
            );
        }

        private static IGrouping<TKey, TElement> Grouping<TKey, TElement>(TKey key, IEnumerable<TElement> elements)
            => HarshGrouping.Create(key, elements);
    }
}
