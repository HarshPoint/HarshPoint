using HarshPoint.Server.Provisioning;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System;

namespace HarshPoint.Server.Tests
{
    public class SharePointServerFixture : IDisposable
    {
        public SharePointServerFixture()
        {
            Site = new SPSite("http://" + Environment.MachineName);
            Web = Site.RootWeb;
            WebApplication = Site.WebApplication;
            Farm = WebApplication.Farm;
            WebContext = new HarshServerProvisionerContext(Site.RootWeb);
        }

        public void Dispose()
        {
            if (Web != null) Web.Dispose();
            if (Site != null) Site.Dispose();

            Web = null;
            Site = null;
            WebApplication = null;
            Farm = null;
            WebContext = null;
        }


        public SPWeb Web { get; set; }
        public SPSite Site { get; set; }
        public SPWebApplication WebApplication { get; set; }
        public SPFarm Farm { get; set; }
        public HarshServerProvisionerContext WebContext { get; set; }
    }
}
