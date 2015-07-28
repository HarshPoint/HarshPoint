using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using System.Collections.Generic;
using System.Linq;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveTermStoreTermSet :
        NestedResolveBuilder<TermSet, TermStore, ClientObjectResolveContext>
    {
        public ResolveTermStoreTermSet(IResolveBuilder<TermStore, ClientObjectResolveContext> parent)
            : base(parent)
        {
        }

        protected override void InitializeContextBeforeParent(ClientObjectResolveContext context)
        {
            context.Include<TermStore>(
                ts => ts.Groups.Include(
                    g => g.TermSets
                )
            );

            base.InitializeContextBeforeParent(context);
        }

        protected override IEnumerable<TermSet> SelectChildren(TermStore parent)
        {
            if (parent == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(parent));
            }

            return parent.Groups.SelectMany(g => g.TermSets);
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<ResolveTermStoreTermSet>();
    }
}
