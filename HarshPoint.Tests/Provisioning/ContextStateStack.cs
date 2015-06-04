using Xunit;

namespace HarshPoint.Tests.Provisioning
{
    public class ContextStateStack : IClassFixture<SharePointClientFixture>
    {
        public ContextStateStack(SharePointClientFixture fixture)
        {
            ClientOM = fixture;
        }

        public SharePointClientFixture ClientOM { get; private set; }

        [Fact]
        public void Context_StateStack_empty_by_default()
        {
            Assert.Empty(ClientOM.Context.StateStack);
        }

        [Fact]
        public void Context_PushStack_adds_object()
        {
            var ctx = ClientOM.Context.PushState("42");

            Assert.Empty(ClientOM.Context.StateStack);
            Assert.Single(ctx.StateStack, "42");
        }
    }
}
