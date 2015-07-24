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

            ExistingField = DeferredResolveBuilder.Create(
                () => Resolve.Field().ById(Id)
            );

            Type = FieldType.Text;

            SchemaXmlBuilder = new HarshFieldSchemaXmlBuilder(

                new HarshFieldSchemaXmlAttributeSetter(() => DisplayName)
                {
                    ValueValidator = ValidateNotNullOrWhitespace,
                },

                new HarshFieldSchemaXmlAttributeSetter(() => Group),

                new HarshFieldSchemaXmlAttributeSetter(() => TypeName)
                {
                    Name = "Type",
                    ValueValidator = ValidateNotNullOrWhitespace,
                },

                // only on create below

                new HarshFieldSchemaXmlAttributeSetter(() => Id)
                {
                    Name = "ID",
                    ValueValidator = ValidateNotEmptyGuid,
                    OnlyOnCreate = true,
                },

                new HarshFieldSchemaXmlAttributeSetter(() => InternalName)
                {
                    Name = "Name",
                    ValueValidator = ValidateNotNullOrWhitespace,
                    OnlyOnCreate = true,
                },

                new HarshFieldSchemaXmlAttributeSetter(() => StaticName)
                {
                    ValueAccessor = () => StaticName ?? InternalName,
                    ValueValidator = ValidateNotNullOrWhitespace,
                    OnlyOnCreate = true
                }
            );
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

        [Parameter]
        public String DisplayName { get; set; }

        public Field Field { get; private set; }

        /// <summary>
        /// Gets or sets the name of the field type.
        /// </summary>
        [Parameter]
        public String TypeName { get; set; }

        [Parameter]
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

        [Parameter]
        [DefaultFromContext(typeof(DefaultFieldGroup))]
        public String Group { get; set; }

        /// <summary>
        /// Gets or sets the field identifier.
        /// </summary>
        /// <value>
        /// The field identifier. Must not be an empty <see cref="Guid"/>.
        /// </value>
        //[Parameter(Mandatory = true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the InternalName of the field.
        /// Only used when creating a new field.
        /// </summary>
        [Parameter]
        public String InternalName { get; set; }

        [Parameter]
        public Boolean PushChangesToLists { get; set; }

        /// <summary>
        /// Gets or sets the StaticName of the field.
        /// Only used when creating a new field.
        /// </summary>
        [Parameter]
        public String StaticName { get; set; }

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            TargetFieldCollection = Web.Fields;
        }

        protected override void InitializeResolveContext(ClientObjectResolveContext context)
        {
            context.Include<Field>(
                f => f.Id,
                f => f.InternalName,
                f => f.SchemaXmlWithResourceTokens
            );

            base.InitializeResolveContext(context);
        }

        protected override async Task OnProvisioningAsync()
        {
            if (ExistingField.Value.IsNull())
            {
                Logger.Information("Adding field {InternalName}, id {Id}", InternalName, Id);

                var schemaXml = SchemaXmlBuilder.Create();

                TargetFieldCollection.AddFieldAsXml(
                    schemaXml.ToString(),
                    AddToDefaultView,
                    AddFieldOptions
                );

                // cannot use the instance returned from AddFieldAsXml,
                // as that is always of type Field, and not the actual
                // subtype.

                var reResolvedField = ManualResolver.ResolveSingle(
                    Resolve.Field().ById(Id)
                );

                await ClientContext.ExecuteQueryAsync();

                Field = reResolvedField.Value;
            }
            else
            {
                Field = ExistingField.Value;

                var existingSchemaXml = XElement.Parse(
                    Field.SchemaXmlWithResourceTokens
                );

                var updatedSchemaXml = SchemaXmlBuilder.Update(
                    existingSchemaXml
                );

                if (SchemaXmlComparer.Equals(existingSchemaXml, updatedSchemaXml))
                {
                    return;
                }

                Field.SchemaXml = updatedSchemaXml.ToString();
                Field.UpdateAndPushChanges(PushChangesToLists);

                await ClientContext.ExecuteQueryAsync();
            }
        }

        protected override async Task OnUnprovisioningAsync()
        {
            if (!ExistingField.Value.IsNull())
            {
                ExistingField.Value.DeleteObject();
                await ClientContext.ExecuteQueryAsync();
            }
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

        private IResolveSingleOrDefault<Field> ExistingField
        {
            get;
            set;
        }

        private static readonly XNodeEqualityComparer SchemaXmlComparer = new XNodeEqualityComparer();

        private static void ValidateNotEmptyGuid(String propertyName, Object value)
        {
            if (Guid.Empty.Equals(value))
            {
                throw Error.InvalidOperationFormat(
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
                throw Error.InvalidOperationFormat(
                    SR.HarshFieldSchemaXmlProvisioner_PropertyWhiteSpace,
                    propertyName
                );
            }
        }
    }
}
