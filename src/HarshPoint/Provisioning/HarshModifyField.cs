using Microsoft.SharePoint.Client;
using System;
using System.Diagnostics.CodeAnalysis;

namespace HarshPoint.Provisioning
{
    public sealed class HarshModifyField : HarshModifyField<Field>
    {
        [Parameter]
        public String DisplayName { get; set; }

        [Parameter]
        [DefaultFromContext(typeof(DefaultFieldGroup))]
        public String Group { get; set; }

        /// <summary>
        /// Gets or sets the name of the field type.
        /// </summary>
        [Parameter(ParameterSetName = "TypeName")]
        public String TypeName { get; set; }

        [Parameter(ParameterSetName = "Type")]
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        public FieldType? Type { get; set; }
    }
}
