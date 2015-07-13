using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class OldResolveListFieldById
        : Implementation.OldNestedResolvable<List, Field, Guid, HarshProvisionerContext, OldResolveListFieldById>
    {
        public OldResolveListFieldById(IResolveOld<List> parent, IEnumerable<Guid> ids)
            : base(parent, ids)
        {
        }

        protected override Task<IEnumerable<Field>> ResolveChainElement(ResolveContext<HarshProvisionerContext> context, List parent)
        {
            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            if (parent == null)
            {
                throw Error.ArgumentNull(nameof(parent));
            }

            return this.ResolveQuery(
                ClientObjectResolveQuery.FieldById,
                context, 
                parent.Fields
            );
        }
    }
}
