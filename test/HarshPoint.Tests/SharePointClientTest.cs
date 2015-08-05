using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using HarshPoint.Provisioning.Output;
using Microsoft.SharePoint.Client;
using System.Collections.Generic;
using System.Linq;
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
        public IReadOnlyCollection<HarshProvisionerOutput> Output => Fixture?.Output;
        public Site Site => Fixture?.Site;
        public Web Web => Fixture?.Web;
        public SharePointClientFixture Fixture { get; private set; }
        public ClientObjectManualResolver ManualResolver { get; private set; }

        protected IdentifiedObjectOutputBase<T> FindOutput<T>()
            => Output.OfType<IdentifiedObjectOutputBase<T>>().Last();
    }
}
