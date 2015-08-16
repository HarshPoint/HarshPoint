using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using System;

namespace HarshPoint.Provisioning
{
    public sealed class HarshProvisionerContext :
        Implementation.HarshProvisionerContextBase<HarshProvisionerContext>
    {
        private TaxonomySession _taxonomySession;

        public HarshProvisionerContext(ClientContext clientContext)
        {
            if (clientContext == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(clientContext));
            }

            ClientContext = clientContext;
        }

        public ClientContext ClientContext
        {
            get;

        }

        public Site Site => ClientContext.Site;

        public TaxonomySession TaxonomySession
            => HarshLazy.Initialize(
                ref _taxonomySession,
                () => TaxonomySession.GetTaxonomySession(ClientContext)
            );

        public Web Web => ClientContext.Web;

        public override String ToString() => ClientContext.Url;

        private static readonly HarshLogger Logger = HarshLog.ForContext<HarshProvisionerContext>();
    }
}
