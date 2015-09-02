using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveFieldById :
        ClientObjectIdentifierResolveBuilder<Field, Guid>
    {
        public ResolveFieldById(IEnumerable<Guid> identifiers)
            : base(identifiers)
        {
        }

        protected override IEnumerable<Func<Guid, Field>> GetSelectors(
            HarshProvisionerContext context
        )
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            yield return id => context.Web.Fields.GetById(id);
            yield return id => context.Web.AvailableFields.GetById(id);
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ResolveFieldById));
    }
}