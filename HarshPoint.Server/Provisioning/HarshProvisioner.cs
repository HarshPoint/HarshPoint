using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace HarshPoint.Server.Provisioning
{
    public abstract class HarshProvisioner
    {
        private SPWeb _web;
        private SPSite _site;
        private SPWebApplication _webApp;
        private SPFarm _farm;

        public SPSite Site
        {
            get { return _site; }
            set
            {
                _web = null;
                _site = value;
                _webApp = (_site != null) ? _site.WebApplication : null;
                _farm = (_webApp != null) ? _webApp.Farm : null;
            }
        }

        public SPWeb Web
        {
            get { return _web; }
            set
            {
                _web = value;
                _site = (_web != null) ? _web.Site : null;
                _webApp = (_site != null) ? _site.WebApplication : null;
                _farm = (_webApp != null) ? _webApp.Farm : null;
            }
        }

        public SPWebApplication WebApplication
        {
            get { return _webApp; }
            set
            {
                _web = null;
                _site = null;
                _webApp = value;
                _farm = (_webApp != null) ? _webApp.Farm : null;
            }
        }

        public SPFarm Farm
        {
            get { return _farm; }
            set
            {
                _web = null;
                _site = null;
                _webApp = null;
                _farm = value;
            }
        }

        public void Provision()
        {
            try
            {
                Initialize();
                OnProvisioning();
            }
            finally
            {
                Complete();
            }
        }

        public void Unprovision()
        {
            try
            {
                Initialize();
                OnUnprovisioning();
            }
            finally
            {
                Complete();
            }
        }

        protected virtual void Initialize()
        {
        }

        protected virtual void Complete()
        {
        }

        protected virtual void OnProvisioning()
        {
        }

        protected virtual void OnUnprovisioning()
        {
        }
    }
}
