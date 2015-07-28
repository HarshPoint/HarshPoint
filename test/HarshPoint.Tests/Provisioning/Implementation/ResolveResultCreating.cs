using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning.Implementation
{
    public class ResolveResultCreating : SeriloggedTest
    {
        public ResolveResultCreating(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [InlineData(typeof(IResolve<String>), typeof(ResolveResult<String>))]
        [InlineData(typeof(IResolveSingle<String>), typeof(ResolveResultSingle<String>))]
        [InlineData(typeof(IResolveSingleOrDefault<String>), typeof(ResolveResultSingleOrDefault<String>))]
        public void Creates_expected_ResolveResult(Type interfaceType, Type resultType)
        {
            var result = ResolveResultFactory.CreateResult(
                interfaceType.GetTypeInfo(),
                new[] { "42" },
                Mock.Of<IResolveBuilder>()
            );
            Assert.IsType(resultType, result);
        }

        [Fact]
        public void Casts_to_derived_type()
        {
            var expected = new DerivedClass();

            var result = (IEnumerable)ResolveResultFactory.CreateResult(
                typeof(IResolve<DerivedClass>).GetTypeInfo(),
                new BaseClass[] { expected },
                Mock.Of<IResolveBuilder>()
            );

            Assert.IsType(typeof(ResolveResult<DerivedClass>), result);

            var actual = Assert.Single(result);
            Assert.Same(expected, actual);
        }


        [Theory]
        [InlineData(typeof(String))]
        [InlineData(typeof(IEnumerable<Int32>))]
        public void Fails_invalid_property_type(Type invalidType)
        {
            Assert.Throws<ArgumentException>(
                () => ResolveResultFactory.CreateResult(
                    invalidType.GetTypeInfo(),
                    new[] { "42" },
                    Mock.Of<IResolveBuilder>()
                )
            );
        }

        [Fact]
        public void Fails_incompatible_IEnumerable()
        {
            var result = (ResolveResultSingle<Int32>)ResolveResultFactory.CreateResult(
                typeof(IResolveSingle<Int32>).GetTypeInfo(),
                new[] { "42" },
                Mock.Of<IResolveBuilder>()
            );

            Assert.Throws<InvalidCastException>(
                () => result.Value
            );
        }

        private class BaseClass { }
        private class DerivedClass : BaseClass { }
    }
}
