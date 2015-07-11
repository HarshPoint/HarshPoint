using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using System;
using System.Collections.Generic;
using Xunit;

namespace HarshPoint.Tests.Provisioning.Implementation
{
    public class ResolveResultConverting
    {
        [Fact]
        public void Creates_ResolveResult()
        {
            var result = ResolveResultConverter.CreateResult(typeof(IResolve<String>), new[] { "42" });
            Assert.IsType<ResolveResult<String>>(result);
        }

        [Theory]
        [InlineData(typeof(String))]
        [InlineData(typeof(IEnumerable<Int32>))]
        public void Fails_invalid_property_type(Type invalidType)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => ResolveResultConverter.CreateResult(invalidType, new[] { "42" })
            );
        }
    }
}
