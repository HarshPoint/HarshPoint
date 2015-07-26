using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using Moq;
using System;
using System.Collections;
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

        private IEnumerable CreateResult<T>(params Object[] source)
            => (IEnumerable)ResolveResultFactory.CreateResult(
                typeof(IResolve<T>).GetTypeInfo(),
                source,
                Mock.Of<IResolveBuilder>()
            );
    }
}
