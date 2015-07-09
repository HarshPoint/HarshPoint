using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using System.Collections.Immutable;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning
{
    public class IdentifierResolving : SharePointClientTest
    {
        public IdentifierResolving(SharePointClientFixture fixture, ITestOutputHelper output) : base(fixture, output)
        {
        }

        [Fact]
        public async Task Failures_are_recorded()
        {
            var items = new[] { "42", "4242", "unused" };
            var ids = new[]
            {
                "1",
                "abc",
                "42",
                "2",
                "def",
                "4242",
                "3",
                "ghi",
            };

            var expectedResults = ids
                .Where(x => items.Contains(x))
                .ToArray();

            var expectedFailures = ids
                .Where(x => !items.Contains(x))
                .ToArray();

            var resolver = new IdResolver(items, ids);
            var ctx = new ResolveContext<HarshProvisionerContext>(Fixture.Context);
            var results = await resolver.TryResolveAsync(ctx);

            Assert.Equal(expectedResults, results);

            Assert.Equal(expectedFailures.Length, ctx.Failures.Count);
            Assert.All(ctx.Failures, fail => Assert.Equal(resolver, fail.Resolvable));
            Assert.Equal(expectedFailures, ctx.Failures.Select(fail => fail.Identifier));
        }

        private sealed class IdResolver : IResolveOld<String>, IResolvableIdentifiers<String>
        {
            private readonly IImmutableList<String> _identifiers;
            private readonly IImmutableList<String> _items;

            public IdResolver(IEnumerable<String> items, IEnumerable<String> identifiers)
            {
                _items = items.ToImmutableArray();
                _identifiers = identifiers.ToImmutableArray();
            }

            public IImmutableList<String> Identifiers => _identifiers;

            public Task<IEnumerable<String>> TryResolveAsync(IResolveContext context)
            {
                return Task.FromResult(
                    this.ResolveItems(
                        context,
                        _items.Select(i => Tuple.Create(i, i))
                    )
                );
            }
        }
    }
}
