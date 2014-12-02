using HarshPoint.Provisioning;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System;

namespace HarshPoint.Server.Provisioning
{
    public abstract class HarshServerProvisioner : HarshProvisionerBase
    {
        public HarshServerProvisionerContext Context
        {
            get;
            set;
        }

        public SPWeb Web
        {
            get { return ContextPropertyOrNull(c => c.Web); }
        }

        public SPSite Site
        {
            get { return ContextPropertyOrNull(c => c.Site); }
        }

        public SPWebApplication WebApplication
        {
            get { return ContextPropertyOrNull(c => c.WebApplication); }
        }

        public SPFarm Farm
        {
            get { return ContextPropertyOrNull(c => c.Farm); }
        }

        private T ContextPropertyOrNull<T>(Func<HarshServerProvisionerContext, T> func)
            where T : class
        {
            if (Context != null)
            {
                return func(Context);
            }

            return null;
        }
    }
}
