using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarshPoint.Entity.Metadata
{
    internal sealed class HarshEntityMetadataContentType : HarshEntityMetadata
    {
        internal HarshEntityMetadataContentType(Type entityType)
            : base(entityType)
        {
        }
    }
}
