using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveFieldByTitleOrInternalName : 
        ClientObjectIdentifierResolveBuilder<Field, String>
    {
        public ResolveFieldByTitleOrInternalName(
            IEnumerable<String> identifiers
        )
            : base(identifiers)
        {
        }

        protected override IEnumerable<Func<String, Field>> GetSelectors(
            HarshProvisionerContext context
        )
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            yield return name => 
                context.Web.Fields.GetByInternalNameOrTitle(name);

            yield return name => 
                context.Web.AvailableFields.GetByInternalNameOrTitle(name);
        }


        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ResolveFieldByTitleOrInternalName));
    }
}