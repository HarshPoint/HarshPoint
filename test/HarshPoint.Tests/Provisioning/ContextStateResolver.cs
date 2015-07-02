using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning
{
    public class ContextStateResolver : SharePointClientTest
    {
        public ContextStateResolver(SharePointClientFixture fixture, ITestOutputHelper output) : base(fixture, output)
        {
        }

        [Fact]
        public async Task Resolves_String()
        {
            var ctx = Fixture.Context.PushState("42");
            var resolver = new ContextStateResolver<String>();

            var resolveCtx = new ResolveContext<HarshProvisionerContext>(ctx);
            var many = await resolver.TryResolveAsync(resolveCtx);

            Assert.Single(many, "42");
        }

        [Fact]
        public async Task Resolves_most_recent_String()
        {
            var ctx = Fixture.Context.PushState("4242").PushState("42");
            var resolver = new ContextStateResolver<String>();

            var resolveCtx = new ResolveContext<HarshProvisionerContext>(ctx);
            var many = await resolver.TryResolveAsync(resolveCtx);

            Assert.Single(many, "42");
        }

        [Fact]
        public async Task Flattens_enumerable_states()
        {
            var ctx = Fixture.Context
                .PushState("123")
                .PushState(new[] { "42", "4242" });

            var resolver = new ContextStateResolver<String>();

            var resolveCtx = new ResolveContext<HarshProvisionerContext>(ctx);
            var many = await resolver.TryResolveAsync(resolveCtx);

            Assert.Collection(
                many,
                x => Assert.Equal("42", x),
                x => Assert.Equal("4242", x),
                x => Assert.Equal("123", x)
            );
        }
    }
}
