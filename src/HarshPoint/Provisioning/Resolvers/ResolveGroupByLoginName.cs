using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Resolvers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Login")]
    public sealed class ResolveGroupByLoginName : ClientObjectIdentifierResolveBuilder<Group, String>
    {
        public ResolveGroupByLoginName(
            IResolveBuilder<Group> parent,
            IEnumerable<String> names
        )
            : base(parent, names, l => l.LoginName)
        {
        }
    }
}
