using Microsoft.SharePoint.Client;
using System;
using System.Threading.Tasks;
using HarshPoint.Provisioning.Implementation;

namespace HarshPoint.Provisioning
{
    public sealed class HarshModifyFieldLookup : HarshModifyField<FieldLookup>
    {
        [Parameter(Mandatory = true)]
        public IResolveSingle<Tuple<List, Field>> LookupTarget
        {
            get;
            set;
        }

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

        protected override async Task OnProvisioningAsync()
        {
            foreach (var field in Fields)
            {
                field.LookupList = LookupTarget.Value.Item1.Id.ToString("B");
                field.LookupField = LookupTarget.Value.Item2.InternalName;
                field.LookupWebId = Web.Id;

                UpdateField(field);
            }

            await ClientContext.ExecuteQueryAsync();
        }
    }
}
