using Microsoft.SharePoint.Client;
using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning
{
    [CLSCompliant(false)]
    public abstract class HarshProvisioner : HarshProvisionerBase
    {
        private ClientContext _ctx;
        private Site _site;
        private Web _web;

        public ClientContext Context
        {
            get { return _ctx; }
            set
            {
                _ctx = value;
                _site = (_ctx != null) ? _ctx.Site : null;
                _web = (_ctx != null) ? _ctx.Web : null;
            }
        }

        public Site Site
        {
            get { return _site; }
            set
            {
                _site = value;
                _ctx = (_site != null) ? (_site.Context as ClientContext) : null;
                _web = (_site != null) ? (_site.RootWeb) : null;
            }
        }

        public Web Web
        {
            get { return _web; }
            set
            {
                _web = value;
                _ctx = (_web != null) ? (_web.Context as ClientContext) : null;
                _site = (_ctx != null) ? _ctx.Site : null;
            }
        }
    }
}
