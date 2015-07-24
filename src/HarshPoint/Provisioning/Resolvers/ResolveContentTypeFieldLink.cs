using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveContentTypeFieldLink :
        NestedResolveBuilder<FieldLink, ContentType, ClientObjectResolveContext>
    {
        public ResolveContentTypeFieldLink(IResolveBuilder<ContentType, ClientObjectResolveContext> parent) 
            : base(parent)
        {
        }

        protected override void InitializeContextBeforeParent(ClientObjectResolveContext context)
        {
            context.Include<ContentType>(
                ct => ct.FieldLinks
            );
        }

        protected override IEnumerable<FieldLink> ToEnumerable(Object state, ClientObjectResolveContext context)
        {
            return Parent.ToEnumerable(state, context).SelectMany(ct => ct.FieldLinks);
        }
    }
}
