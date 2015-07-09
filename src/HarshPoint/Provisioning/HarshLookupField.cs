using Microsoft.SharePoint.Client;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning
{
    public class HarshLookupField : HarshFieldProvisioner<FieldLookup>
    {
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public IResolveOld<Tuple<List, Field>> LookupTarget
        {
            get;
            set;
        }

        protected override async Task OnProvisioningAsync()
        {
            ClientContext.Load(Web, w => w.Id);
            await ClientContext.ExecuteQueryAsync();

            var lookupField = await ResolveSingleAsync(
                LookupTarget
                .Include(field => field.InternalName)
                .IncludeOnParent(list => list.Id)
            );
            
            foreach (var field in FieldsResolved)
            {
                field.LookupList = lookupField.Item1.Id.ToString("B");
                field.LookupField = lookupField.Item2.InternalName;
                field.LookupWebId = Web.Id;

                UpdateField(field);
            }

            await ClientContext.ExecuteQueryAsync();
        }
    }
}
