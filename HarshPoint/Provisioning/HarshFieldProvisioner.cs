using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace HarshPoint.Provisioning
{
    public sealed class HarshFieldProvisioner : HarshFieldProvisionerBase
    {
        private readonly HarshFieldSchemaXmlBuilder SchemaXmlBuilder;

        public HarshFieldProvisioner()
        {
            SchemaXmlBuilder = new HarshFieldSchemaXmlBuilder()
            {
                Transformers =
                {
                    new NonNullAttributeSetter(() => FieldTypeName, "Type"),
                    new NonNullAttributeSetter(() => InternalName, onFieldAddOnly: true),
                    new NonNullAttributeSetter(() => StaticName, onFieldAddOnly: true),
                }
            };
        }

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

        public String FieldTypeName
        {
            get;
            set;
        }

        public String InternalName
        {
            get;
            set;
        }

        public String StaticName
        {
            get;
            set;
        }

        public XElement SchemaXml
        {
            get;
            set;
        }

        public Collection<HarshFieldSchemaXmlTransformer> SchemaXmlTransformers
        {
            get { return SchemaXmlBuilder.Transformers; }
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
            FieldAdded = false;
            FieldRemoved = false;
            FieldUpdated = false;

            base.Initialize();
        }

        protected override void OnProvisioning()
        {
            base.OnProvisioning();

            SchemaXml = SchemaXmlBuilder.Update(Field, SchemaXml);

            if (Field.IsNull())
            {
                Field = TargetFieldCollection.AddFieldAsXml(SchemaXml.ToString(), AddToDefaultView, AddFieldOptions);
                Context.ExecuteQuery();

                FieldAdded = true;
            }
            else
            {
                Field.SchemaXml = SchemaXml.ToString();
                Context.ExecuteQuery();

                FieldUpdated = true;
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

        private sealed class NonNullAttributeSetter : HarshFieldSchemaXmlTransformer
        {
            private readonly XName _name;
            private readonly Func<Object> _valueAccessor;

            public NonNullAttributeSetter(Expression<Func<Object>> valueAccessorExpr, XName name = null, Boolean onFieldAddOnly = false)
            {
                if (name == null)
                {
                    _name = valueAccessorExpr.GetMemberName();
                }
                else
                {
                    _name = name;
                }

                _valueAccessor = valueAccessorExpr.Compile();
                OnFieldAddOnly = OnFieldAddOnly;
            }

            public override XElement Transform(XElement element)
            {
                var value = _valueAccessor();

                if (value != null)
                {
                    element.SetAttributeValue(_name, value);
                }

                return element;
            }
        }
    }
}
