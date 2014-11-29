using Microsoft.SharePoint.Publishing;
using System;

namespace HarshPoint.Server.Provisioning
{
    [CLSCompliant(false)]
    public static class HarshProvisionerPublishing
    {
        public static PublishingWeb ValidateIsPublishingWeb(this HarshServerProvisioner provisioner)
        {
            if (provisioner == null)
            {
                throw Error.ArgumentNull("provisioner");
            }

            if (!PublishingWeb.IsPublishingWeb(provisioner.Web))
            {
                throw Error.InvalidOperation(
                    SR.HarshProvisionerPublishing_NotAPublishingWeb, provisioner.Web.Url
                );
            }

            return PublishingWeb.GetPublishingWeb(provisioner.Web);
        }
    }
}
