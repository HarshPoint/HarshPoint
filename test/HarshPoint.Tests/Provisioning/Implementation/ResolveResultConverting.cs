using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning.Implementation
{
    public class ResolveResultConverting : SeriloggedTest
    {
        public ResolveResultConverting(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Gets_unpacked()
        {
            var result = CreateResult<String>(
                NestedResolveResult.Pack("42", "parent")
            );

            var actual = Assert.Single(result);
            Assert.Equal("42", actual);
        }

        [Fact]
        public void Gets_converted_to_Tuple()
        {
            var result = CreateResult<Tuple<String, Int32>>(
                NestedResolveResult.Pack(42, "test")
            );

            var actual = Assert.Single(result);
            Assert.Equal(Tuple.Create("test", 42), actual);
        }

        [Fact]
        public void Gets_converted_to_Tuple_skipping_some_components()
        {
            var result = CreateResult<Tuple<String, Int32>>(
                NestedResolveResult.Pack(42,
                    NestedResolveResult.Pack(true, "test")
                )
            );

            var actual = Assert.Single(result);
            Assert.Equal(Tuple.Create("test", 42), actual);
        }

        [Fact]
        public void Gets_converted_to_Tuple_of_Tuples()
        {
            var result = CreateResult<Tuple<String, Tuple<Boolean, Int32>>>(
                NestedResolveResult.Pack(42,
                    NestedResolveResult.Pack(true, "test")
                )
            );

            var actual = Assert.Single(result);
            Assert.Equal(Tuple.Create("test", Tuple.Create(true, 42)), actual);
        }

        [Fact]
        public void Root_Grouping_gets_converted()
        {
            var results = CreateResult<IGrouping<Int32, String>>(
                NestedResolveResult.Pack("hello", 42),
                NestedResolveResult.Pack("and now for something", 4321),
                NestedResolveResult.Pack("there", 42),
                NestedResolveResult.Pack("completely different", 4321)
            );

            Assert.Equal(2, results.Count());

            var first = Assert.Single(results, r => r.Key == 42);
            var second = Assert.Single(results, r => r.Key == 4321);

            Assert.Equal(new[] { "hello", "there" }, first);
            Assert.Equal(new[] { "and now for something", "completely different" }, second);
        }

        [Fact]
        public void Root_Grouping_with_tuple_element_gets_converted()
        {
            var results = CreateResult<IGrouping<Int32, Tuple<Int32, String>>>(
                NestedResolveResult.Pack("hello", NestedResolveResult.Pack(2, 42)),
                NestedResolveResult.Pack("and now for something", NestedResolveResult.Pack(2, 4321)),
                NestedResolveResult.Pack("there", NestedResolveResult.Pack(2, 42)),
                NestedResolveResult.Pack("completely different", NestedResolveResult.Pack(2, 4321))
            );

            Assert.Equal(2, results.Count());

            var first = Assert.Single(results, r => r.Key == 42);
            var second = Assert.Single(results, r => r.Key == 4321);

            Assert.Equal(new[] 
            {
                Tuple.Create(2, "hello"),
                Tuple.Create(2, "there" )
            }, first);

            Assert.Equal(new[] 
            {
                Tuple.Create(2, "and now for something"),
                Tuple.Create(2, "completely different" )
            }, second);
        }


        [Fact]
        public void Root_Grouping_with_tuple_keys_gets_converted()
        {
            var results = CreateResult<IGrouping<Tuple<Int32, Int32>, String>>(
                NestedResolveResult.Pack("hello", NestedResolveResult.Pack(2, 42)),
                NestedResolveResult.Pack("and now for something", NestedResolveResult.Pack(2, 4321)),
                NestedResolveResult.Pack("there", NestedResolveResult.Pack(2, 42)),
                NestedResolveResult.Pack("completely different", NestedResolveResult.Pack(2, 4321))
            ).ToArray();

            Assert.Equal(2, results.Count());

            var first = Assert.Single(results, r => r.Key.Equals(Tuple.Create(42, 2)));
            var second = Assert.Single(results, r => r.Key.Equals(Tuple.Create(4321, 2)));

            Assert.Equal(new[] { "hello", "there" }, first);
            Assert.Equal(new[] { "and now for something", "completely different" }, second);
        }

        [Fact]
        public void Nested_Grouping_gets_converted()
        {
            var results = CreateResult<IGrouping<Int32, IGrouping<Int32, String>>>(
                NestedResolveResult.Pack("hello", NestedResolveResult.Pack(2, 42)),
                NestedResolveResult.Pack("and now for something", NestedResolveResult.Pack(2, 4321)),
                NestedResolveResult.Pack("there", NestedResolveResult.Pack(2, 42)),
                NestedResolveResult.Pack("completely different", NestedResolveResult.Pack(2, 4321))
            );

            Assert.Equal(2, results.Count());

            var first = Assert.Single(results, r => r.Key == 42);
            var second = Assert.Single(results, r => r.Key == 4321);

            var first2 = Assert.Single(first, g => g.Key == 2);
            var second2 = Assert.Single(second, g => g.Key == 2);

            Assert.Equal(new[] { "hello", "there" }, first2);
            Assert.Equal(new[] { "and now for something", "completely different" }, second2);
        }

        private IEnumerable<T> CreateResult<T>(params Object[] source)
            => (IEnumerable<T>)ResolveResultFactory.CreateResult(
                typeof(IResolve<T>).GetTypeInfo(),
                source,
                Mock.Of<IResolveBuilder>()
            );
    }
}
