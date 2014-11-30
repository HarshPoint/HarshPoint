using HarshPoint.Provisioning;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System;

namespace HarshPoint.Server.Provisioning
{
    public abstract class HarshServerProvisioner : HarshProvisionerBase
    {
        private SPWeb _web;
        private SPSite _site;
        private SPWebApplication _webApp;
        private SPFarm _farm;

        /// <summary>
        /// The <see cref="Microsoft.SharePoint.SPWeb"/> instance 
        /// upon which the provisioner will act.
        /// </summary>
        /// <remarks>
        /// Please note that setting this property will automatically assign
        /// the parent objects to <see cref="Site"/>, <see cref="WebApplication"/>,
        /// and <see cref="Farm"/> properties.
        /// </remarks>
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

        /// <summary>
        /// The <see cref="Microsoft.SharePoint.SPSite"/> instance 
        /// upon which the provisioner will act.
        /// </summary>
        /// <remarks>
        /// <para>Please note that setting this property will automatically assign
        /// the parent objects to <see cref="WebApplication"/> and <see cref="Farm"/>
        /// properties, and will set the <see cref="Web"/> property to the root web 
        /// of the site collection.</para>
        /// </remarks>
        public SPSite Site
        {
            get { return _site; }
            set
            {
                _site = value;
                _web = (_site != null) ? _site.RootWeb : null;
                _webApp = (_site != null) ? _site.WebApplication : null;
                _farm = (_webApp != null) ? _webApp.Farm : null;
            }
        }

        /// <summary>
        /// The <see cref="Microsoft.SharePoint.Administration.SPWebApplication"/>
        /// instance upon which the provisioner will act.
        /// </summary>
        /// <remarks>
        /// <para>Please note that setting this property will automatically assign
        /// the parent object to the <see cref="Farm"/> property, and will set 
        /// the <see cref="Web"/> and <see cref="Site"/> properties to <c>null</c>.</para>
        /// </remarks>
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

        /// <summary>
        /// The <see cref="Microsoft.SharePoint.Administration.SPFarm"/>
        /// instance upon which the provisioner will act.
        /// </summary>
        /// <remarks>
        /// <para>Please note that setting this property will automatically assign
        /// the <see cref="Web"/>, <see cref="Site"/>, and <see cref="WebApplication"/> 
        /// properties to <c>null</c>.</para>
        /// </remarks>
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

        internal void CopyContextFrom(HarshServerProvisioner other)
        {
            if (other == null)
            {
                throw Error.ArgumentNull("other");
            }

            _farm = other.Farm;
            _webApp = other.WebApplication;
            _site = other.Site;
            _web = other.Web;
        }
    }
}
