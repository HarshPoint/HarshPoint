using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using System;
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
        public void Resolves_String()
        {
            var ctx = Fixture.Context.PushState("42");
            var resolver = new ContextStateResolver<String>();

            var mr = new ManualResolver(() => new ResolveContext<HarshProvisionerContext>(ctx));
            var many = mr.Resolve(resolver);

            Assert.Single(many, "42");
        }

        [Fact]
        public void Resolves_most_recent_String()
        {
            var ctx = Fixture.Context.PushState("4242").PushState("42");
            var resolver = new ContextStateResolver<String>();

            var mr = new ManualResolver(() => new ResolveContext<HarshProvisionerContext>(ctx));
            var many = mr.Resolve(resolver);

            Assert.Single(many, "42");
        }

        [Fact]
        public void Flattens_enumerable_states()
        {
            var ctx = Fixture.Context
                .PushState("123")
                .PushState(new[] { "42", "4242" });

            var resolver = new ContextStateResolver<String>();

            var mr = new ManualResolver(() => new ResolveContext<HarshProvisionerContext>(ctx));
            var many = mr.Resolve(resolver);

            Assert.Collection(
                many,
                x => Assert.Equal("42", x),
                x => Assert.Equal("4242", x),
                x => Assert.Equal("123", x)
            );
        }
    }
}
