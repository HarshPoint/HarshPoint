using Microsoft.SharePoint.Client;
using System;
using System.Diagnostics.CodeAnalysis;

namespace HarshPoint.Provisioning
{
    [DefaultParameterSet(nameof(Type))]
    public sealed class HarshModifyFieldType :
        HarshModifyField<Field, HarshModifyFieldType>
    {
        public HarshModifyFieldType()
        {
            Map(f => f.TypeAsString).From(p => p.TypeName);
            Map(f => f.FieldTypeKind).From(p => p.Type);
        }

        /// <summary>
        /// Gets or sets the name of the field type.
        /// </summary>
        [Parameter(ParameterSetName = nameof(TypeName))]
        public String TypeName { get; set; }

        [Parameter(ParameterSetName = nameof(Type))]
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        public FieldType? Type { get; set; }
    }
}
