using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests
{
    public class TupleTests : SeriloggedTest
    {
        public TupleTests(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [InlineData(typeof(Tuple<>))]
        [InlineData(typeof(Tuple<Int32>))]
        [InlineData(typeof(Tuple<,>))]
        [InlineData(typeof(Tuple<String, String>))]
        [InlineData(typeof(Tuple<,,>))]
        [InlineData(typeof(Tuple<Boolean, Int32, Object>))]
        [InlineData(typeof(Tuple<,,,>))]
        [InlineData(typeof(Tuple<,,,,>))]
        [InlineData(typeof(Tuple<,,,,,>))]
        [InlineData(typeof(Tuple<,,,,,,>))]
        [InlineData(typeof(Tuple<,,,,,,,>))]
        public void IsTupleType_is_true_for_tuples(Type type)
        {
            Assert.True(HarshTuple.IsTupleType(type));
        }

        [Theory]
        [InlineData(typeof(String))]
        [InlineData(typeof(IEnumerable<>))]
        public void IsTupleType_is_false_for_non_tuples(Type type)
        {
            Assert.False(HarshTuple.IsTupleType(type));
        }

        [Theory]
        [InlineData(typeof(Tuple<Int32>), new[] { typeof(Int32) })]
        [InlineData(typeof(Tuple<Int32, String>), new[] { typeof(Int32), typeof(String) })]
        [InlineData(
            typeof(Tuple<Q, Q, Q, Q, Q, Q, Q, Q>), 
            new[] { typeof(Q), typeof(Q), typeof(Q), typeof(Q), typeof(Q), typeof(Q), typeof(Q), typeof(Q) })
        ]
        public void MakeTupleType_makes_tuple_type(Type expected, params Type[] args)
        {
            Assert.Equal(expected, HarshTuple.MakeTupleType(args));
        }

        [Fact]
        public void MakeTupleType_fails_empty()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => HarshTuple.MakeTupleType()
            );
        }

        [Fact]
        public void MakeTupleType_creates_nested_tuple_nine()
        {
            var actual = HarshTuple.MakeTupleType(
                typeof(Q),
                typeof(Q),
                typeof(Q),

                typeof(Q),
                typeof(Q),
                typeof(Q),

                typeof(Q),
                typeof(Q),
                typeof(Q)
            );

            Assert.Equal(
                typeof(Tuple<Q, Q, Q, Q, Q, Q, Q, Tuple<Q, Q>>),
                actual
            );
        }

        [Fact]
        public void MakeTupleType_creates_nested_tuple_eighteen()
        {
            var actual = HarshTuple.MakeTupleType(
                typeof(Q), typeof(Q), typeof(Q),
                typeof(Q), typeof(Q), typeof(Q),
                typeof(Q), typeof(Q), typeof(Q),

                typeof(Q), typeof(Q), typeof(Q),
                typeof(Q), typeof(Q), typeof(Q),
                typeof(Q), typeof(Q), typeof(Q)
            );

            Assert.Equal(
                typeof(Tuple<
                    Q, Q, Q, Q, Q, Q, Q,
                    Tuple<
                        Q, Q, Q, Q, Q, Q, Q,
                        Tuple<Q, Q, Q, Q>
                    >
                >),
                actual
            );
        }

        private sealed class Q { }
    }
}
