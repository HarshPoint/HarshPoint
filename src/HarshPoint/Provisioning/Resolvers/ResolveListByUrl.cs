﻿using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Implementation
{
    public sealed class ResolveListByUrl : IdentifierResolveBuilder<List, ClientObjectResolveContext, String>
    {
        public ResolveListByUrl(
            IResolveBuilder<List> parent,
            IEnumerable<String> urls
        )
            : base(parent, urls, StringComparer.OrdinalIgnoreCase)
        {
        }

        protected override void InitializeContextBeforeParent(ClientObjectResolveContext context)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            context.Include<List>(
                list => list.ParentWebUrl,
                list => list.RootFolder.ServerRelativeUrl
            );

            base.InitializeContextBeforeParent(context);
        }

        protected override String GetIdentifier(List result)
        {
            if (result == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(result));
            }

            return HarshUrl.GetRelativeTo(result.RootFolder.ServerRelativeUrl, result.ParentWebUrl);
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<ResolveListByUrl>();
    }
}
