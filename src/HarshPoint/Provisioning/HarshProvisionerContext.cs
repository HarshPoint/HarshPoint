﻿using Microsoft.SharePoint.Client;
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
                throw Logger.Fatal.ArgumentNull(nameof(clientContext));
            }

            ClientContext = clientContext;
        }

        public ClientContext ClientContext
        {
            get;
            private set;
        }

        public Site Site => ClientContext.Site;

        public TaxonomySession TaxonomySession 
            => HarshLazy.Initialize(
                ref _taxonomySession, 
                () => TaxonomySession.GetTaxonomySession(ClientContext)
            );

        public Web Web => ClientContext.Web;

        private static readonly HarshLogger Logger = HarshLog.ForContext<HarshProvisionerContext>();
    }
}
