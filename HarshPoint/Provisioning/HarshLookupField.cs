﻿using Microsoft.SharePoint.Client;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning
{
    public class HarshLookupField : HarshFieldProvisioner<FieldLookup>
    {
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public IResolve<Tuple<List, Field>> LookupTarget
        {
            get;
            set;
        }

        protected override async Task<HarshProvisionerResult> OnProvisioningAsync()
        {
            var lookupField = await ResolveSingleAsync(
                LookupTarget
            );
            
            foreach (var field in FieldsResolved)
            {
                field.LookupList = lookupField.Item1.Id.ToString("B");
                field.LookupField = lookupField.Item2.Id.ToString("B");
                field.LookupWebId = Web.Id;

                UpdateField(field);
            }

            await ClientContext.ExecuteQueryAsync();

            return await base.OnProvisioningAsync();
        }
    }
}