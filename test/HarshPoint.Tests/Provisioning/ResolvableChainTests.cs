using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning
{
    [Obsolete]
    public class ResolvableChainTests : SharePointClientTest
    {
        public ResolvableChainTests(SharePointClientFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
        }

        [Fact]
        public async Task TryResolve_returns_no_results()
        {
            IResolveOld<String> chain = new DummyChain(null, null);

            var ctx = new ResolveContext<HarshProvisionerContext>(Fixture.Context);
            var actual = await chain.TryResolveAsync(ctx);

            Assert.Empty(actual);
            ctx.ValidateNoFailures();
        }

        [Fact]
        public async Task TryResolve_returns_one_result()
        {
            var expected = "one";
            IResolveOld<String> chain = new DummyChain(new[] { expected }, null);

            var ctx = new ResolveContext<HarshProvisionerContext>(Fixture.Context);
            var actual = await chain.TryResolveAsync(ctx);
            Assert.Single(actual, expected);
            ctx.ValidateNoFailures();
        }

        [Fact]
        public async Task TryResolve_returns_many_results()
        {
            var expected = new[] { "one", "two" };
            IResolveOld<String> chain = new DummyChain(expected, null);

            var ctx = new ResolveContext<HarshProvisionerContext>(Fixture.Context);
            var actual = await chain.TryResolveAsync(ctx);
            Assert.Equal(expected, actual);
            ctx.ValidateNoFailures();
        }

        [Fact]
        public async Task TryResolve_returns_many_results_and_failures()
        {
            var expectedResults = new[] { "one", "two" };
            var expectedFails = new[] { "fail1", "fail2" };

            IResolveOld<String> chain = new DummyChain(expectedResults, expectedFails);

            var ctx = new ResolveContext<HarshProvisionerContext>(Fixture.Context);
            var actual = await chain.TryResolveAsync(ctx);
            Assert.Equal(expectedResults, actual);

            Assert.All(ctx.Failures, f => Assert.Same(chain, f.Resolvable));
            Assert.Equal(expectedFails, ctx.Failures.Select(f => f.Identifier));
        }

        [Fact]
        public async Task TryResolveSingle_returns_no_results()
        {
            IResolveOld<String> chain = new DummyChain(null, null);

            var ctx = new ResolveContext<HarshProvisionerContext>(Fixture.Context);
            var actual = await chain.TryResolveSingleAsync(ctx);

            Assert.Null(actual);
            ctx.ValidateNoFailures();
        }

        [Fact]
        public async Task TryResolveSingle_returns_one_result()
        {
            var expected = "one";
            IResolveOld<String> chain = new DummyChain(new[] { expected }, null);

            var ctx = new ResolveContext<HarshProvisionerContext>(Fixture.Context);
            var actual = await chain.TryResolveSingleAsync(ctx);
            Assert.Equal(expected, actual);
            ctx.ValidateNoFailures();
        }

        [Fact]
        public async Task TryResolveSingle_returns_first_of_many_results()
        {
            var expected = new[] { "one", "two" };
            IResolveOld<String> chain = new DummyChain(expected, null);

            var ctx = new ResolveContext<HarshProvisionerContext>(Fixture.Context);
            var actual = await chain.TryResolveSingleAsync(ctx);
            Assert.Equal(expected[0], actual);
            ctx.ValidateNoFailures();
        }

        [Fact]
        public async Task TryResolveSingle_returns_first_of_many_results_and_reports_failures()
        {
            var expectedResults = new[] { "one", "two" };
            var expectedFails = new[] { "fail1", "fail2" };

            IResolveOld<String> chain = new DummyChain(expectedResults, expectedFails);

            var ctx = new ResolveContext<HarshProvisionerContext>(Fixture.Context);
            var actual = await chain.TryResolveSingleAsync(ctx);
            Assert.Equal(expectedResults[0], actual);

            Assert.All(ctx.Failures, f => Assert.Same(chain, f.Resolvable));
            Assert.Equal(expectedFails, ctx.Failures.Select(f => f.Identifier));
        }

        [Fact]
        public async Task Resolve_returns_one_result()
        {
            var expected = "one";
            IResolveOld<String> chain = new DummyChain(new[] { expected }, null);

            var ctx = new ResolveContext<HarshProvisionerContext>(Fixture.Context);
            var actual = await chain.ResolveAsync(ctx);
            Assert.Single(actual, expected);
            ctx.ValidateNoFailures();
        }

        [Fact]
        public async Task Resolve_returns_many_results()
        {
            var expected = new[] { "one", "two" };
            IResolveOld<String> chain = new DummyChain(expected, null);

            var ctx = new ResolveContext<HarshProvisionerContext>(Fixture.Context);
            var actual = await chain.ResolveAsync(ctx);
            Assert.Equal(expected, actual);
            ctx.ValidateNoFailures();
        }

        [Fact]
        public async Task Resolve_returns_no_results()
        {
            IResolveOld<String> chain = new DummyChain(null, null);

            var ctx = new ResolveContext<HarshProvisionerContext>(Fixture.Context);
            var actual = await chain.ResolveAsync(ctx);

            Assert.Empty(actual);
            ctx.ValidateNoFailures();
        }

        [Fact]
        public async Task Resolve_throws_on_failure()
        {

            var expectedResults = new[] { "one", "two" };
            var expectedFails = new[] { "fail1", "fail2" };

            IResolveOld<String> chain = new DummyChain(expectedResults, expectedFails);

            var ctx = new ResolveContext<HarshProvisionerContext>(Fixture.Context);

            var exc = await Assert.ThrowsAsync<ResolveFailedException>(() => chain.ResolveAsync(ctx));

            Assert.All(exc.Failures, f => Assert.Same(chain, f.Resolvable));
            Assert.Equal(expectedFails, exc.Failures.Select(f => f.Identifier));
        }

        [Fact]
        public async Task ResolveSingle_throws_on_no_results()
        {
            IResolveOld<String> chain = new DummyChain(null, null);

            var ctx = new ResolveContext<HarshProvisionerContext>(Fixture.Context);
            await Assert.ThrowsAsync<InvalidOperationException>(() => chain.ResolveSingleAsync(ctx));
        }

        [Fact]
        public async Task ResolveSingle_returns_one_result()
        {
            var expected = "one";
            IResolveOld<String> chain = new DummyChain(new[] { expected }, null);

            var ctx = new ResolveContext<HarshProvisionerContext>(Fixture.Context);
            var actual = await chain.ResolveSingleAsync(ctx);
            Assert.Equal(expected, actual);
            ctx.ValidateNoFailures();
        }

        [Fact]
        public async Task ResolveSingle_throws_on_many_results()
        {
            var expected = new[] { "one", "two" };
            IResolveOld<String> chain = new DummyChain(expected, null);

            var ctx = new ResolveContext<HarshProvisionerContext>(Fixture.Context);
            await Assert.ThrowsAsync<InvalidOperationException>(() => chain.ResolveSingleAsync(ctx));
        }

        [Fact]
        public async Task ResolveSingle_throws_on_failures()
        {
            var expectedResults = new[] { "one" };
            var expectedFails = new[] { "fail1", "fail2" };

            IResolveOld<String> chain = new DummyChain(expectedResults, expectedFails);

            var ctx = new ResolveContext<HarshProvisionerContext>(Fixture.Context);
            var exc = await Assert.ThrowsAsync<ResolveFailedException>(() => chain.ResolveSingleAsync(ctx));

            Assert.All(exc.Failures, f => Assert.Same(chain, f.Resolvable));
            Assert.Equal(expectedFails, exc.Failures.Select(f => f.Identifier));
        }

        [Fact]
        public void And_clones_this()
        {
            var dummy = new DummyChain();
            var other = new DummyChain();
            var result = dummy.And(other);

            Assert.NotSame(dummy, result);
            Assert.NotSame(dummy, other);
        }

        [Fact]
        public async Task And_clones_other()
        {
            var dummy = new DummyChain();
            var other = new DummyChain();
            var combined = (IResolveOld<String>)dummy.And(other);

            other.Results = new[] { "aaa" };

            var actual = await combined.TryResolveAsync(Fixture.ResolveContext);
            Assert.Empty(actual);
        }

        [Fact]
        public async Task Resolve_combined_returns_results_from_both()
        {
            var dummy = new DummyChain(new[] { "aaa" });
            var other = new DummyChain(new[] { "bbb" });
            var combined = (IResolveOld<String>)dummy.And(other);

            var result = await combined.TryResolveAsync(Fixture.ResolveContext);

            Assert.Equal(2, result.Count());
            Assert.Contains("aaa", result);
            Assert.Contains("bbb", result);
        }

        private sealed class DummyChain : ResolvableChain, IResolvableChainElementOld<String>, IResolveOld<String>
        {
            public DummyChain(String[] results = null, String[] failures = null)
            {
                Failures = failures ?? new String[0];
                Results = results ?? new String[0];
            }

            public String[] Failures
            {
                get;
                private set;
            }

            public String[] Results
            {
                get;
                set;
            }

            public DummyChain And(DummyChain other)
            {
                return (DummyChain)base.And(other);
            }

            public override ResolvableChain Clone()
            {
                var clone = (DummyChain)base.Clone();
                clone.Failures  = (String[])Failures.Clone();
                clone.Results = (String[])Results.Clone();
                return clone;
            }

            public Task<IEnumerable<String>> ResolveChainElementOld(IResolveContext context)
            {
                if (Failures != null)
                {
                    foreach (var fail in Failures)
                    {
                        context.AddFailure(this, fail);
                    }
                }

                return Task.FromResult<IEnumerable<String>>(Results);
            }

            Task<IEnumerable<String>> IResolveOld<String>.TryResolveAsync(IResolveContext context)
            {
                return ResolveChainOld<String>(context);
            }
        }
    }
}
