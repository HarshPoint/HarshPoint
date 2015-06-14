using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using System;

namespace HarshPoint.Tests
{
    public class SharePointClientFixture
    {
        public SharePointClientFixture()
        {
            var url = Environment.GetEnvironmentVariable("HarshPointTestUrl");

            if (String.IsNullOrWhiteSpace(url))
            {
                ClientContext = new ClientContext("http://" + Environment.MachineName);
            }
            else
            {
                ClientContext = new ClientContext(url);
                ClientContext.Credentials = new SharePointOnlineCredentials(
                    Environment.GetEnvironmentVariable("HarshPointTestUser"),
                    Environment.GetEnvironmentVariable("HarshPointTestPassword")
                );
            }

            Context = new HarshProvisionerContext(ClientContext);
        }

        public HarshProvisionerContext Context
        {
            get;
            private set;
        }

        public ResolveContext<HarshProvisionerContext> ResolveContext
            => new ResolveContext<HarshProvisionerContext>(Context);

        public ClientContext ClientContext
        {
            get;
            set;
        }

        public Site Site
        {
            get { return ClientContext?.Site; }
        }

        public Web Web
        {
            get { return ClientContext?.Web; }
        }

        public TaxonomySession TaxonomySession
            => Context?.TaxonomySession;
    }
}
