using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveListFieldByInternalName :
        OldNestedResolvable<List, Field, String, HarshProvisionerContext, ResolveListFieldByInternalName>
    {
        public ResolveListFieldByInternalName(IResolveOld<List> parent, IEnumerable<String> names)
            : base(parent, names)
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
                ClientObjectResolveQuery.FieldByInternalName,
                context,
                parent.Fields
            );
        }
    }
}
