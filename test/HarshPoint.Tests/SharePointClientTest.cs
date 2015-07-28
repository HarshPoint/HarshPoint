using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests
{
    public abstract class SharePointClientTest :
        SeriloggedTest,
        IClassFixture<SharePointClientFixture>
    {
        public SharePointClientTest(SharePointClientFixture fixture, ITestOutputHelper output)
            : base(output)
        {
            Fixture = fixture;
            ManualResolver = new ClientObjectManualResolver(fixture.CreateResolveContext);
        }

        public ClientContext ClientContext => Fixture?.ClientContext;
        public Site Site => Fixture?.Site;
        public Web Web => Fixture?.Web;

        public SharePointClientFixture Fixture { get; private set; }

        public ClientObjectManualResolver ManualResolver { get; private set; }
    }
}
