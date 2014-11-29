using Microsoft.SharePoint.Client;
using System;

namespace HarshPoint.Tests
{
    public class SharePointClientFixture
    {
        public SharePointClientFixture()
        {
            Context = new ClientContext("http://" + Environment.MachineName);
        }

        public ClientContext Context
        {
            get;
            set;
        }

        public Site Site
        {
            get { return (Context != null) ? Context.Site : null; }
        }

        public Web Web
        {
            get { return (Context != null) ? Context.Web : null; }
        }
    }
}
