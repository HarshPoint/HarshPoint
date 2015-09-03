using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using Moq;
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
            var result = new ResolveResult<String>()
            {
                Results = new String[0],
                ResolveContext = MockResolveContext(),
            };
            Assert.Empty(result);
        }

        [Fact]
        public void ResolveResult_returns_single_result()
        {
            var result = new ResolveResult<String>()
            {
                Results = new[] { "42" },
                ResolveContext = MockResolveContext(),
            };

            Assert.Single(result, "42");
        }

        [Fact]
        public void ResolveResult_returns_more_results()
        {
            var result = new ResolveResult<Int32>()
            {
                Results = Enumerable.Repeat(42, 3),
                ResolveContext = MockResolveContext(),
            };
            Assert.Equal(Enumerable.Repeat(42, 3), result);
        }

        [Fact]
        public void ResolveResult_throws_on_failures()
        {
            var result = new ResolveResult<Int32>()
            {
                Results = new[] { 42 },
                ResolveContext = MockResolveContext("4242"),
            };

            var exc = Assert.Throws<ResolveFailedException>(() => result.ToArray());
            var rf = Assert.Single(exc.Failures);
            Assert.Equal("4242", rf.Identifier);
        }


        [Fact]
        public void ResolveResultSingle_returns_single_result()
        {
            var result = new ResolveResultSingle<Int32>()
            {
                Results = Enumerable.Repeat(42, 1),
                ResolveContext = MockResolveContext(),
            };
            Assert.Equal(42, result.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("abc")]
        public void ResolveResultSingle_fails(IEnumerable<Char> chars)
        {
            var result = new ResolveResultSingle<Char>()
            {
                Results = chars,
                ResolveContext = MockResolveContext(),
            };
            Assert.Throws<InvalidOperationException>(() => result.Value);
        }


        [Fact]
        public void ResolveResultSingle_throws_on_failures()
        {
            var result = new ResolveResultSingle<Int32>()
            {
                Results = new[] { 42 },
                ResolveContext = MockResolveContext("4242"),
            };

            var exc = Assert.Throws<ResolveFailedException>(() => result.Value);
            var rf = Assert.Single(exc.Failures);
            Assert.Equal("4242", rf.Identifier);
        }

        [Fact]
        public void ResolveResultSingleOrDefault_returns_no_results()
        {
            var result = new ResolveResultSingleOrDefault<Int32>()
            {
                Results = new Int32[0],
                ResolveContext = MockResolveContext(),
            };
            Assert.Equal(0, result.Value);
            Assert.False(result.HasValue);
        }

        [Fact]
        public void ResolveResultSingleOrDefault_returns_single_result()
        {
            var result = new ResolveResultSingleOrDefault<Int32>()
            {
                Results = Enumerable.Repeat(42, 1),
                ResolveContext = MockResolveContext(),
            };
            Assert.Equal(42, result.Value);
            Assert.True(result.HasValue);
        }

        [Fact]
        public void ResolveResultSingleOrDefault_fails_multiple_results()
        {
            var result = new ResolveResultSingleOrDefault<Char>()
            {
                Results = "1234",
                ResolveContext = MockResolveContext(),
            };
            Assert.Throws<InvalidOperationException>(() => result.Value);
            Assert.Throws<InvalidOperationException>(() => result.HasValue);
        }

        [Fact]
        public void ResolveResultSingleOrDefault_ignores_failures()
        {
            var result = new ResolveResultSingleOrDefault<Int32>()
            {
                Results = new[] { 42 },
                ResolveContext = MockResolveContext("4242"),
            };

            Assert.Equal(42, result.Value);
            Assert.True(result.HasValue);
        }

        private ResolveContext MockResolveContext(params Object[] failedIds)
        {
            var pctx = Mock.Of<IHarshProvisionerContext>();
            var mock = new Mock<ResolveContext>(pctx);

            foreach (var id in failedIds)
            {
                mock.Object.AddFailure(Mock.Of<IResolveBuilder>(), id);
            }

            return mock.Object;
        }
    }
}
