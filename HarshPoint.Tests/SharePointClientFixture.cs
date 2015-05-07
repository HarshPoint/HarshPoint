using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using System;

namespace HarshPoint.Tests
{
    public class SharePointClientFixture
    {
        public SharePointClientFixture()
        {
            ClientContext = new ClientContext("http://" + Environment.MachineName);
            Context = new HarshProvisionerContext(ClientContext);
        }

        public HarshProvisionerContext Context
        {
            get;
            private set;
        }

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
    }
}
