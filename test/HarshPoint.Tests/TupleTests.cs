using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
        [InlineData(typeof(String))]
        [InlineData(typeof(IEnumerable<>))]
        [InlineData(typeof(IEnumerable<Int32>))]
        public void GetComponentTypes_fails_for_non_tuple(Type t)
        {
            Assert.Throws<ArgumentException>(
                () => HarshTuple.GetComponentTypes(t)
            );
        }

        [Theory]
        [InlineData(typeof(Tuple<>))]
        [InlineData(typeof(Tuple<,>))]
        [InlineData(typeof(Tuple<,,>))]
        [InlineData(typeof(Tuple<,,,>))]
        [InlineData(typeof(Tuple<,,,,>))]
        [InlineData(typeof(Tuple<,,,,,>))]
        [InlineData(typeof(Tuple<,,,,,,>))]
        [InlineData(typeof(Tuple<,,,,,,,>))]
        public void GetComponentTypes_fails_for_unconstructed_tuple(Type t)
        {
            Assert.Throws<ArgumentException>(
                () => HarshTuple.GetComponentTypes(t)
            );
        }

        [Theory]
        [InlineData(typeof(Tuple<String>), new[] { typeof(String) })]
        [InlineData(typeof(Tuple<Int32, String>), new[] { typeof(Int32), typeof(String) })]
        [InlineData(typeof(Tuple<Int32, String, Boolean>), new[] { typeof(Int32), typeof(String), typeof(Boolean) })]
        public void GetComponentTypes_returns_components(Type t, Type[] expected)
        {
            Assert.Equal(expected, HarshTuple.GetComponentTypes(t));
        }

        [Fact]
        public void GetComponentTypes_returns_components_nine_Qs()
        {
            Assert.Equal(
                NineQs,
                HarshTuple.GetComponentTypes(NineQsType)
            );
        }

        [Fact]
        public void GetComponentTypes_returns_components_eighteen_Qs()
        {
            Assert.Equal(
                EighteenQs,
                HarshTuple.GetComponentTypes(EighteenQsType)
            );
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
        [InlineData(typeof(IEnumerable<Int32>))]
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
            Assert.Throws<ArgumentException>(
                () => HarshTuple.MakeTupleType()
            );
        }

        [Fact]
        public void MakeTupleType_creates_nested_tuple_nine()
        {
            var actual = HarshTuple.MakeTupleType(NineQs);
            Assert.Equal(NineQsType, actual);
        }

        [Fact]
        public void MakeTupleType_creates_nested_tuple_eighteen()
        {
            var actual = HarshTuple.MakeTupleType(EighteenQs);
            Assert.Equal(EighteenQsType, actual);
        }

        private static readonly ImmutableArray<Type> NineQs = ImmutableArray.Create(
            typeof(Q), typeof(Q), typeof(Q),
            typeof(Q), typeof(Q), typeof(Q),
            typeof(Q), typeof(Q), typeof(Q)
        );

        private static readonly ImmutableArray<Type> EighteenQs = ImmutableArray.Create(
            typeof(Q), typeof(Q), typeof(Q),
            typeof(Q), typeof(Q), typeof(Q),
            typeof(Q), typeof(Q), typeof(Q),

            typeof(Q), typeof(Q), typeof(Q),
            typeof(Q), typeof(Q), typeof(Q),
            typeof(Q), typeof(Q), typeof(Q)
        );

        private static readonly Type NineQsType
            = typeof(Tuple<Q, Q, Q, Q, Q, Q, Q, Tuple<Q, Q>>);

        private static readonly Type EighteenQsType
        = typeof(Tuple<
                Q, Q, Q, Q, Q, Q, Q,
                Tuple<
                    Q, Q, Q, Q, Q, Q, Q,
                    Tuple<Q, Q, Q, Q>
                >
            >);

        private sealed class Q { }
    }
}
