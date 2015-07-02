using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning
{
    public class ContextStateStack : SharePointClientTest
    {
        public ContextStateStack(SharePointClientFixture fixture, ITestOutputHelper output) : base(fixture, output)
        {
        }

        [Fact]
        public void Context_StateStack_empty_by_default()
        {
            Assert.Empty(Fixture.Context.StateStack);
        }

        [Fact]
        public void Context_PushStack_adds_object()
        {
            var ctx = Fixture.Context.PushState("42");

            Assert.Empty(Fixture.Context.StateStack);
            Assert.Single(ctx.StateStack, "42");
        }
    }
}
