using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using HarshPoint.Provisioning.Output;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests
{
    public abstract class SharePointClientTest :
        SeriloggedTest
    {
        public SharePointClientTest(ITestOutputHelper output)
            : base(output)
        {
            Fixture = new SharePointClientFixture();
            ManualResolver = new ClientObjectManualResolver(CreateResolveContext);

            var listSink = new HarshProvisionerOutputSinkList();
            Output = listSink.Output;

            Context = new HarshProvisionerContext(ClientContext)
                .WithOutputSink(
                    new HarshProvisionerOutputSinkComposite(
                        new HarshProvisionerOutputSinkSerilog(HarshLog.ForContext("ProvisionerOutput", true)),
                        listSink
                    )
                );
        }

        public override void Dispose()
        {
            Fixture.Dispose();
            base.Dispose();
        }

        public HarshProvisionerContext Context { get; set; }
        public ClientContext ClientContext => Fixture.ClientContext;
        public IReadOnlyCollection<HarshProvisionerOutput> Output { get; set; }
        public Site Site => ClientContext.Site;
        public TaxonomySession TaxonomySession => Context.TaxonomySession;
        public Web Web => ClientContext.Web;
        public SharePointClientFixture Fixture { get; private set; }
        public ClientObjectManualResolver ManualResolver { get; private set; }


        protected virtual ClientObjectResolveContext CreateResolveContext()
            => new ClientObjectResolveContext(Context);

        protected IdentifiedObjectOutputBase<T> FindOutput<T>()
            => Output.OfType<IdentifiedObjectOutputBase<T>>().Last();
    }
}
