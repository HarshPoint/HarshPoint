using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace HarshPoint.Provisioning
{
    /// <summary>
    /// Creates or updates a SharePoint field by generating its schema XML.
    /// </summary>
    public sealed class HarshFieldProvisioner : HarshFieldProvisionerBase
    {
        private readonly HarshFieldSchemaXmlBuilder SchemaXmlBuilder;
        private readonly XNodeEqualityComparer SchemaXmlComparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="HarshFieldProvisioner"/> class.
        /// </summary>
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
            SchemaXmlComparer = new XNodeEqualityComparer();
        }

        /// <summary>
        /// Gets or sets the <see cref="AddFieldOptions"/> value used when
        /// creating a new field.
        /// </summary>
        public AddFieldOptions AddFieldOptions
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to add the newly created field to the default view.
        /// </summary>
        /// <value>
        /// <c>true</c> if add the newly created field to the default view; otherwise, <c>false</c>.
        /// </value>
        public Boolean AddToDefaultView
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the field type.
        /// </summary>
        public String FieldTypeName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the InternalName of the field.
        /// Only used when creating a new field.
        /// </summary>
        public String InternalName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the StaticName of the field.
        /// Only used when creating a new field.
        /// </summary>
        public String StaticName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the field schema XML. If <c>null</c>,
        /// the existing schema XML will be modified. If <c>null</c> and
        /// the field doesn't exist yet, a schema XML will be generated
        /// from the other properties.
        /// </summary>
        public XElement SchemaXml
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the collection of schema XML transformers run when
        /// creating and/or updating a field.
        /// </summary>
        public Collection<HarshFieldSchemaXmlTransformer> SchemaXmlTransformers
        {
            get { return SchemaXmlBuilder.Transformers; }
        }

        /// <summary>
        /// Gets a value indicating whether a field was created during the last operation.
        /// </summary>
        public Boolean FieldAdded
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether a field was removed during the last operation.
        /// </summary>
        public Boolean FieldRemoved
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether a field was updated during the last operation.
        /// </summary>
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
                var existingSchemaXml = SchemaXmlBuilder.GetExistingSchemaXml(Field);

                if (!SchemaXmlComparer.Equals(existingSchemaXml, SchemaXml))
                {
                    Field.SchemaXml = SchemaXml.ToString();
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

        private sealed class NonNullAttributeSetter : HarshFieldSchemaXmlTransformer
        {
            private readonly XName _name;
            private readonly Func<Object> _valueAccessor;

            public NonNullAttributeSetter(Expression<Func<Object>> valueAccessorExpr, XName name = null, Boolean onFieldAddOnly = false)
            {
                if (valueAccessorExpr == null)
                {
                    throw Error.ArgumentNull("valueAccessorExpr");
                }

                if (name == null)
                {
                    _name = valueAccessorExpr.GetMemberName();
                }
                else
                {
                    _name = name;
                }

                _valueAccessor = valueAccessorExpr.Compile();
                OnFieldAddOnly = onFieldAddOnly;
            }

            public override XElement Transform(XElement element)
            {
                if (element == null)
                {
                    throw Error.ArgumentNull("element");
                }

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
