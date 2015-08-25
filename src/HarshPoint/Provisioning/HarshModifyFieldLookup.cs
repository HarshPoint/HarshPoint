using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;

namespace HarshPoint.Provisioning
{
    public sealed class HarshModifyFieldLookup :
        HarshModifyField<FieldLookup, HarshModifyFieldLookup>
    {
        public HarshModifyFieldLookup()
        {
            Map(f => f.LookupList)
                .From(p => p.LookupTarget.Value.Item1.Id.ToStringInvariant("B"))
                .When(p => p.LookupTarget != null);

            Map(f => f.LookupField)
                .From(p => p.LookupTarget.Value.Item2.InternalName)
                .When(p => p.LookupTarget != null);

            Map(f => f.LookupWebId).From(p => p.Web.Id);
        }

        [Parameter(Mandatory = true)]
        public IResolveSingle<Tuple<List, Field>> LookupTarget { get; set; }

        protected override void InitializeResolveContext(ClientObjectResolveContext context)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            context.Load(Web, w => w.Id);

            context.Include<Field>(
                f => f.InternalName
            );

            context.Include<List>(
                list => list.Id
            );

            base.InitializeResolveContext(context);
        }
    }
}
