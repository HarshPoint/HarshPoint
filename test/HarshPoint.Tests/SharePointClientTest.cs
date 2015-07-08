using Microsoft.SharePoint.Client;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests
{
    public abstract class SharePointClientTest :
        SeriloggedTest, 
        IClassFixture<SharePointClientFixture>
     //   IClassFixture<SystemNetLoggingFixture>
    {
        public SharePointClientTest(SharePointClientFixture fixture, ITestOutputHelper output) 
            : base(output)
        {
            Fixture = fixture;
        }

        public ClientContext ClientContext => Fixture?.ClientContext;
        public Site Site => Fixture?.Site;
        public Web Web => Fixture?.Web;

        public SharePointClientFixture Fixture { get; private set; }
    }
}
