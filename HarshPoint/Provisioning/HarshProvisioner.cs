using Microsoft.SharePoint.Client;
using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning
{
    /// <summary>
    /// Provides context for classes provisioning SharePoint
    /// artifacts using the client-side object model.
    /// </summary>
    public abstract class HarshProvisioner : HarshProvisionerBase
    {
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
