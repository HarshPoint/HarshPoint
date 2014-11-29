using Microsoft.SharePoint.Client;
using System;
using System.Xml.Linq;

namespace HarshPoint.Provisioning
{
    public class HarshFieldProvisioner : HarshFieldProvisionerBase
    {
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

        public XElement FieldSchemaXml
        {
            get;
            set;
        }

        public Boolean FieldAdded
        {
            get;
            private set;
        }

        public Boolean FieldRemoved
        {
            get;
            private set;
        }

        public Boolean FieldUpdated
        {
            get;
            private set;
        }

        protected override void Initialize()
        {
            base.Initialize();

            FieldAdded = false;
            FieldRemoved = false;
            FieldUpdated = false;
        }

        protected override void OnProvisioning()
        {
            base.OnProvisioning();

            if (Field.IsNull())
            {
                if (FieldSchemaXml == null)
                {
                    throw Error.InvalidOperation(SR.HarshFieldProvisioner_SchemaXmlNotSet);
                }

                Field = TargetFieldCollection.AddFieldAsXml(FieldSchemaXml.ToString(), AddToDefaultView, AddFieldOptions);
                Context.ExecuteQuery();

                FieldAdded = true;
            }
            else
            {
                if (FieldSchemaXml != null)
                {
                    Field.SchemaXml = FieldSchemaXml.ToString();
                    Context.ExecuteQuery();

                    FieldUpdated = true;
                }
            }
        }

        protected override void OnUnprovisioningMayDeleteUserData()
        {
            if (!Field.IsNull())
            {
                Field.DeleteObject();
                Context.ExecuteQuery();

                FieldRemoved = true;
            }

            base.OnUnprovisioningMayDeleteUserData();
        }
    }
}
