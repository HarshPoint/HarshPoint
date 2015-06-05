using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HarshPoint.Provisioning
{
    /// <summary>
    /// Creates or updates a SharePoint field by generating its schema XML.
    /// </summary>
    public sealed class HarshField : HarshFieldProvisioner
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HarshField"/> class.
        /// </summary>
        public HarshField()
        {
            ModifyChildrenContextState(() => Field);

            TypeName = "Text";
            SchemaXmlBuilder = new HarshFieldSchemaXmlBuilder()
            {
                Transformers =
                {
                    new HarshFieldSchemaXmlAttributeSetter(() => DisplayName)
                    {
                        ValueValidator = ValidateNotNullOrWhitespace,
                    },

                    new HarshFieldSchemaXmlAttributeSetter(() => Group),

                    new HarshFieldSchemaXmlAttributeSetter(() => Id)
                    {
                        Name = "ID",
                        ValueValidator = ValidateNotEmptyGuid,
                        SkipWhenModifying = true,
                    },

                    new HarshFieldSchemaXmlAttributeSetter(() => InternalName)
                    {
                        Name = "Name",
                        ValueValidator = ValidateNotNullOrWhitespace,
                        SkipWhenModifying = true,
                    },

                    new HarshFieldSchemaXmlAttributeSetter(() => StaticName)
                    {
                        ValueAccessor = () => StaticName ?? InternalName,
                        ValueValidator = ValidateNotNullOrWhitespace,
                        SkipWhenModifying = true
                    },

                    new HarshFieldSchemaXmlAttributeSetter(() => TypeName)
                    {
                        Name = "Type",
                        ValueValidator = ValidateNotNullOrWhitespace,
                    },
                }
            };
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

        public String DisplayName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the field type.
        /// </summary>
        public String TypeName
        {
            get;
            set;
        }

        [DefaultFromContext(typeof(DefaultFieldGroup))]
        public String Group
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

        public Boolean PushChangesToLists
        {
            get;
            private set;
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

        protected override Task InitializeAsync()
        {
            FieldAdded = false;
            FieldRemoved = false;
            FieldUpdated = false;

            return base.InitializeAsync();
        }

        protected override async Task OnProvisioningAsync()
        {
            SchemaXml = await SchemaXmlBuilder.Update(Field, SchemaXml);

            if (Field.IsNull())
            {
                Field = TargetFieldCollection.AddFieldAsXml(
                    SchemaXml.ToString(),
                    AddToDefaultView,
                    AddFieldOptions
                );

                await ClientContext.ExecuteQueryAsync();
                FieldAdded = true;
            }
            else
            {
                var existingSchemaXml = await SchemaXmlBuilder.GetExistingSchemaXml(Field);

                if (!SchemaXmlComparer.Equals(existingSchemaXml, SchemaXml))
                {
                    Field.SchemaXml = SchemaXml.ToString();
                    Field.UpdateAndPushChanges(PushChangesToLists);

                    await ClientContext.ExecuteQueryAsync();
                    FieldUpdated = true;
                }
            }

            await base.OnProvisioningAsync();
        }

        protected override async Task OnUnprovisioningAsync()
        {
            if (!Field.IsNull())
            {
                Field.DeleteObject();
                await ClientContext.ExecuteQueryAsync();

                FieldRemoved = true;
            }

            await base.OnUnprovisioningAsync();
        }

        internal HarshFieldSchemaXmlBuilder SchemaXmlBuilder
        {
            get;
            private set;
        }

        private static readonly XNodeEqualityComparer SchemaXmlComparer = new XNodeEqualityComparer();

        private static void ValidateNotEmptyGuid(String propertyName, Object value)
        {
            if (Guid.Empty.Equals(value))
            {
                throw Error.InvalidOperation(
                    SR.HarshFieldSchemaXmlProvisioner_PropertyEmptyGuid,
                    propertyName
                );
            }
        }

        private static void ValidateNotNullOrWhitespace(String propertyName, Object value)
        {
            var asString = Convert.ToString(value, CultureInfo.InvariantCulture);

            if (value == null || String.IsNullOrWhiteSpace(asString))
            {
                throw Error.InvalidOperation(
                    SR.HarshFieldSchemaXmlProvisioner_PropertyWhiteSpace,
                    propertyName
                );
            }
        }
    }
}
