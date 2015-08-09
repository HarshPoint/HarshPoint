using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning
{
    public class ContextStateStack : SharePointClientTest
    {
        public ContextStateStack(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Context_StateStack_empty_by_default()
        {
            Assert.Empty(Context.StateStack);
        }

        [Fact]
        public void Context_PushStack_adds_object()
        {
            var ctx = Context.PushState("42");

            Assert.Empty(Context.StateStack);
            Assert.Single(ctx.StateStack, "42");
        }
    }
}
