﻿using HarshPoint.Provisioning.Implementation;
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
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            context.Include<ContentType>(
                ct => ct.FieldLinks
            );
        }

        protected override IEnumerable<FieldLink> SelectChildren(ContentType parent)
            => parent.FieldLinks;

        private static readonly HarshLogger Logger = HarshLog.ForContext<ResolveContentTypeFieldLink>();
    }
}