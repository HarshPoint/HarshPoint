using Microsoft.SharePoint.Publishing;

namespace HarshPoint.Server.Provisioning
{
    public static class HarshProvisionerPublishing
    {
        public static PublishingWeb ValidateIsPublishingWeb(this HarshProvisioner provisioner)
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
