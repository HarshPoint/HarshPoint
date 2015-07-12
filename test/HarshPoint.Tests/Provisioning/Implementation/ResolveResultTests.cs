using HarshPoint.Provisioning.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning.Implementation
{
    public class ResolveResultTests : SeriloggedTest
    {
        public ResolveResultTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void ResolveResult_returns_no_results()
        {
            var result = new ResolveResult<String>() { Results = new String[0] };
            Assert.Empty(result);
        }

        [Fact]
        public void ResolveResult_returns_single_result()
        {
            var result = new ResolveResult<String>() { Results = new[] { "42" } };
            Assert.Single(result, "42");
        }

        [Fact]
        public void ResolveResult_returns_more_results()
        {
            var result = new ResolveResult<Int32>() { Results = Enumerable.Repeat(42, 3) };
            Assert.Equal(Enumerable.Repeat(42, 3), result);
        }

        [Fact]
        public void ResolveResultSingle_returns_single_result()
        {
            var result = new ResolveResultSingle<Int32>() { Results = Enumerable.Repeat(42, 1) };
            Assert.Equal(42, result.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("abc")]
        public void ResolveResultSingle_fails(IEnumerable<Char> chars)
        {
            var result = new ResolveResultSingle<Char>() { Results = chars };
            Assert.Throws<InvalidOperationException>(() => result.Value);
        }

        [Fact]
        public void ResolveResultSingleOrDefault_returns_no_results()
        {
            var result = new ResolveResultSingleOrDefault<Int32>() { Results = new Int32[0] };
            Assert.Equal(0, result.Value);
        }

        [Fact]
        public void ResolveResultSingleOrDefault_returns_single_result()
        {
            var result = new ResolveResultSingleOrDefault<Int32>() { Results = Enumerable.Repeat(42, 1) };
            Assert.Equal(42, result.Value);
        }

        [Fact]
        public void ResolveResultSingleOrDefault_fails_multiple_results()
        {
            var result = new ResolveResultSingleOrDefault<Char>() { Results = "1234" };
            Assert.Throws<InvalidOperationException>(() => result.Value);
        }
    }
}
