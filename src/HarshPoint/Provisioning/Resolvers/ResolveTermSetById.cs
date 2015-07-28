using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client.Taxonomy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveTermSetById :
        IdentifierResolveBuilder<TermSet, ClientObjectResolveContext, Guid>
    {
        public ResolveTermSetById(IResolveBuilder<TermSet> parent, IEnumerable<Guid> identifiers)
            : base(parent, identifiers)
        {
        }

        protected override void InitializeContextBeforeParent(ClientObjectResolveContext context)
        {
            context.Include<TermSet>(
                ts => ts.Id
            );

            base.InitializeContextBeforeParent(context);
        }

        protected override Guid GetIdentifier(TermSet result)
        {
            if (result == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(result));
            }

            return result.Id;
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<ResolveTermSetById>();
    }
}
