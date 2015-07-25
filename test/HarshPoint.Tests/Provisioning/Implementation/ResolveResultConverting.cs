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
        public void NestedResolveResult_gets_unpacked()
        {
            var result = CreateResult<String>(
                NestedResolveResult.Pack("42", "parent")
            );

            var actual = Assert.Single(result);
            Assert.Equal("42", actual);
        }


        private IEnumerable CreateResult<T>(params Object[] source)
            => (IEnumerable)ResolveResultFactory.CreateResult(
                typeof(IResolve<T>).GetTypeInfo(),
                source,
                Mock.Of<IResolveBuilder>()
            );
    }
}
