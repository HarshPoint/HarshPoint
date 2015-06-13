using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using System;
using System.Threading.Tasks;
using Xunit;

namespace HarshPoint.Tests.Provisioning
{
    public class ContextStateResolver : IClassFixture<SharePointClientFixture>
    {
        public ContextStateResolver(SharePointClientFixture fixture)
        {
            ClientOM = fixture;
        }

        public SharePointClientFixture ClientOM { get; private set; }

        [Fact]
        public async Task Resolves_String()
        {
            var ctx = ClientOM.Context.PushState("42");
            var resolver = new ContextStateResolver<String>();

            var resolveCtx = new ResolveContext<HarshProvisionerContext>(ctx);
            var single = await resolver.ResolveSingleAsync(resolveCtx);
            var many = await resolver.ResolveAsync(resolveCtx);

            Assert.Equal("42", single);
            Assert.Single(many, "42");
        }

        [Fact]
        public async Task Resolves_most_recent_String()
        {
            var ctx = ClientOM.Context.PushState("4242").PushState("42");
            var resolver = new ContextStateResolver<String>();

            var resolveCtx = new ResolveContext<HarshProvisionerContext>(ctx);
            var single = await resolver.ResolveSingleAsync(resolveCtx);
            var many = await resolver.ResolveAsync(resolveCtx);

            Assert.Equal("42", single);
            Assert.Single(many, "42");
        }

        [Fact]
        public async Task Flattens_enumerable_states()
        {
            var ctx = ClientOM.Context
                .PushState("123")
                .PushState(new[] { "42", "4242" });

            var resolver = new ContextStateResolver<String>();

            var resolveCtx = new ResolveContext<HarshProvisionerContext>(ctx);
            var single = await resolver.ResolveSingleAsync(resolveCtx);
            var many = await resolver.ResolveAsync(resolveCtx);

            Assert.Equal("42", single);
            Assert.Collection(
                many,
                x => Assert.Equal("42", x),
                x => Assert.Equal("4242", x),
                x => Assert.Equal("123", x)
            );
        }
    }
}
