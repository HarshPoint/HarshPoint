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
        }

        /// <summary>
        /// Gets or sets the <see cref="AddFieldOptions"/> value used when
        /// creating a new field.
        /// </summary>
        public AddFieldOptions AddFieldOptions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to add the newly created field to the default view.
        /// </summary>
        /// <value>
        /// <c>true</c> if add the newly created field to the default view; otherwise, <c>false</c>.
        /// </value>
        public Boolean AddToDefaultView { get; set; }

        /// <summary>
        /// Gets or sets the name of the field type.
        /// </summary>
        [MandatoryWhenCreating]
        [Parameter(ParameterSetName = "TypeName")]
        public String TypeName { get; set; }

        [MandatoryWhenCreating]
        [Parameter(ParameterSetName = "Type")]
        public FieldType? Type { get; set; }

        /// <summary>
        /// Gets or sets the field identifier.
        /// </summary>
        /// <value>
        /// The field identifier. Must not be an empty <see cref="Guid"/>.
        /// </value>
        [Parameter(
#if false // fix when DefaultValuePolicy is implemented
            Mandatory = true
#endif
        )]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the InternalName of the field.
        /// Only used when creating a new field.
        /// </summary>
        [Parameter]
        [MandatoryWhenCreating]
        public String InternalName { get; set; }

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

        protected override async Task OnProvisioningAsync()
        {
            if (ExistingField.Value.IsNull())
            {
                ValidateMandatoryWhenCreatingParameters();

                Logger.Information("Adding field {InternalName}, id {Id}", InternalName, Id);

                var schemaXml = BuildSchemaXml();

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

                WriteOutput(
                    Result.Created(InternalName ?? Id.ToString(), Field)
                );
            }
            else
            {
                Field = ExistingField.Value;

                WriteOutput(
                    Result.AlreadyExists(InternalName ?? Id.ToString(), Field)
                );
            }
        }

        private XElement BuildSchemaXml()
        {
            var type = TypeName ?? Type.ToString();

            return new XElement("Field",
                new XAttribute("ID", Id.ToString()),
                new XAttribute("Name", InternalName),
                new XAttribute("Type", type),
                new XAttribute("StaticName", StaticName ?? InternalName)
            );
        }

        protected override async Task OnUnprovisioningAsync()
        {
            if (!ExistingField.Value.IsNull())
            {
                ExistingField.Value.DeleteObject();
                await ClientContext.ExecuteQueryAsync();
            }
        }

        private IResolveSingleOrDefault<Field> ExistingField { get; set; }

        private Field Field { get; set; }

        private FieldCollection TargetFieldCollection { get; set; }
    }
}
