using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace HarshPoint.Tests.Provisioning
{
    public class ResolvableChainTests : IClassFixture<SharePointClientFixture>
    {
        public ResolvableChainTests(SharePointClientFixture fixture)
        {
            ClientOM = fixture;
        }

        public SharePointClientFixture ClientOM { get; private set; }

        [Fact]
        public async Task Resolve_returns_no_results()
        {
            IResolve<String> chain = new DummyChain();

            var actual = await chain.ResolveAsync(ClientOM.ResolveContext);
            Assert.Empty(actual);
        }

        [Fact]
        public async Task Resolve_returns_one_result()
        {
            var expected = "one";
            IResolve<String> chain = new DummyChain(expected);

            var actual = await chain.ResolveAsync(ClientOM.ResolveContext);
            Assert.Equal(expected, Assert.Single(actual));
        }

        [Fact]
        public async Task Resolve_returns_many_results()
        {
            var expected = new[] { "one", "two" };
            IResolve<String> chain = new DummyChain(expected);

            var actual = await chain.ResolveAsync(ClientOM.ResolveContext);
            Assert.Equal(expected, actual);
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
            var combined = (IResolve<String>)dummy.And(other);

            other.Results = new[] { "aaa" };

            var actual = await combined.ResolveAsync(ClientOM.ResolveContext);
            Assert.Empty(actual);
        }

        [Fact]
        public async Task Resolve_combined_returns_results_from_both()
        {
            var dummy = new DummyChain("aaa");
            var other = new DummyChain("bbb");
            var combined = (IResolve<String>)dummy.And(other);

            var result = await combined.ResolveAsync(ClientOM.ResolveContext);

            Assert.Equal(2, result.Count());
            Assert.Contains("aaa", result);
            Assert.Contains("bbb", result);
        }

        private sealed class DummyChain : ResolvableChain, IResolvableChainElement<String>, IResolve<String>
        {
            public DummyChain(params String[] results)
            {
                Results = results;
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
                clone.Results = (String[])Results.Clone();
                return clone;
            }

            public Task<IEnumerable<String>> ResolveChainElement(IResolveContext context)
            {
                return Task.FromResult<IEnumerable<String>>(Results);
            }

            Task<IEnumerable<String>> IResolve<String>.ResolveAsync(IResolveContext context)
            {
                return ResolveChain<String>(context);
            }
        }
    }
}
