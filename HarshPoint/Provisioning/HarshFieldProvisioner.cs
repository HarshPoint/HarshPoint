using Microsoft.SharePoint.Client;
using System;

namespace HarshPoint.Provisioning
{
    public class HarshFieldProvisioner : HarshFieldProvisionerBase
    {
        [CLSCompliant(false)]
        public AddFieldOptions AddFieldOptions
        {
            get;
            set;
        }

        public Boolean AddToDefaultView
        {
            get;
            set;
        }

        public String FieldSchemaXml
        {
            get;
            set;
        }

        protected override void OnProvisioning()
        {
            base.OnProvisioning();

            if (Field.IsNull())
            {
                Field = TargetFieldCollection.AddFieldAsXml(FieldSchemaXml, AddToDefaultView, AddFieldOptions);
            }
            else
            {
                Field.SchemaXml = FieldSchemaXml;
            }

            Context.ExecuteQuery();
        }

        protected override void OnUnprovisioningMayDeleteUserData()
        {
            if (!Field.IsNull())
            {
                Field.DeleteObject();
                Context.ExecuteQuery();
            }

            base.OnUnprovisioningMayDeleteUserData();
        }
    }
}
