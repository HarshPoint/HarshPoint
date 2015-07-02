using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;

namespace HarshPoint.Provisioning
{
    public sealed class HarshProvisionerContext 
        : Implementation.HarshProvisionerContextBase<HarshProvisionerContext>
    {
        private TaxonomySession _taxonomySession;

        public HarshProvisionerContext(ClientContext clientContext)
        {
            if (clientContext == null)
            {
                throw Error.ArgumentNull(nameof(clientContext));
            }

            ClientContext = clientContext;
        }

        public ClientContext ClientContext
        {
            get;
            private set;
        }

        public Site Site => ClientContext?.Site;

        public TaxonomySession TaxonomySession 
            => HarshLazy.Initialize(ref _taxonomySession, CreateTaxonomySession);

        public Web Web => ClientContext?.Web;

        private TaxonomySession CreateTaxonomySession()
            => TaxonomySession.GetTaxonomySession(ClientContext);
    }
}
