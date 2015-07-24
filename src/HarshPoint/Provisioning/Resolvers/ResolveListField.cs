using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveListField : NestedResolveBuilder<Field, List, ClientObjectResolveContext>
    {
        public ResolveListField(IResolveBuilder<List, ClientObjectResolveContext> parent)
            : base(parent)
        {
        }

        protected override void InitializeContextBeforeParent(ClientObjectResolveContext context)
        {
            if(context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            context.Include<List>(
                list => list.Fields
            );
        }

        protected override IEnumerable<Field> ToEnumerable(Object state, ClientObjectResolveContext context)
            => Parent.ToEnumerable(state, context).SelectMany(list => list.Fields);

        private static readonly HarshLogger Logger = HarshLog.ForContext<ResolveListField>();
    }
}
