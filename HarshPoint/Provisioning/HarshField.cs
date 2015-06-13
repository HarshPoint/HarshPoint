using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HarshPoint.Provisioning
{
    /// <summary>
    /// Creates or updates a SharePoint field by generating its schema XML.
    /// </summary>
    public sealed class HarshField : HarshProvisioner
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HarshField"/> class.
        /// </summary>
        public HarshField()
        {
            ModifyChildrenContextState(() => Field);

            Type = FieldType.Text;
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

        public FieldType Type
        {
            get
            {
                FieldType result;

                if (Enum.TryParse(TypeName, ignoreCase: true, result: out result))
                {
                    return result;
                }

                return FieldType.Invalid;
            }
            set
            {
                TypeName = value.ToString();
            }
        }

        [DefaultFromContext(typeof(DefaultFieldGroup))]
        public String Group
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the field identifier.
        /// </summary>
        /// <value>
        /// The field identifier. Must not be an empty <see cref="Guid"/>.
        /// </value>
        public Guid Id
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

        public Field Field
        {
            get;
            private set;
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

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            if (Id == Guid.Empty)
            {
                throw Error.InvalidOperation(SR.HarshFieldProvisionerBase_FieldIdEmpty);
            }

            TargetFieldCollection = Web.Fields;
            Field = await ResolveSingleOrDefaultAsync(Resolve.FieldById(Id));
        }

        protected override async Task<HarshProvisionerResult> OnProvisioningAsync()
        {
            SchemaXml = await SchemaXmlBuilder.Update(Field, SchemaXml);

            if (Field.IsNull())
            {
                TargetFieldCollection.AddFieldAsXml(
                    SchemaXml.ToString(),
                    AddToDefaultView,
                    AddFieldOptions
                );
                await ClientContext.ExecuteQueryAsync();

                // we need to load the field once more, because the 
                // instance returned by AddFieldAsXml is always a Field,
                // not the concerete subtype (e.g. TaxonomyField).

                Field = await ResolveSingleAsync(Resolve.FieldById(Id));
                return ResultFactory.Added(Field);
            }
            else
            {
                var existingSchemaXml = await SchemaXmlBuilder.GetExistingSchemaXml(Field);

                if (!SchemaXmlComparer.Equals(existingSchemaXml, SchemaXml))
                {
                    Field.SchemaXml = SchemaXml.ToString();
                    Field.UpdateAndPushChanges(PushChangesToLists);

                    await ClientContext.ExecuteQueryAsync();

                    return ResultFactory.Updated(Field);
                }
            }

            return ResultFactory.Unchanged(Field);
        }

        protected override async Task<HarshProvisionerResult> OnUnprovisioningAsync()
        {
            if (!Field.IsNull())
            {
                Field.DeleteObject();
                await ClientContext.ExecuteQueryAsync();

                return ResultFactory.Removed(Field.InternalName);
            }

            return await base.OnUnprovisioningAsync();
        }

        internal HarshFieldSchemaXmlBuilder SchemaXmlBuilder
        {
            get;
            private set;
        }

        private FieldCollection TargetFieldCollection
        {
            get;
            set;
        }

        private static readonly HarshProvisionerObjectResultFactory<Field, String> ResultFactory =
            new HarshProvisionerObjectResultFactory<Field, String>(f => f.InternalName);

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
