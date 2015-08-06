using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public FieldType? Type { get; set; }
    }
}
