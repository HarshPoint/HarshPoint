using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HarshPoint.Entity.Metadata
{
    internal sealed class HarshEntityMetadataContentType : HarshEntityMetadata
    {
        internal HarshEntityMetadataContentType(Type entityType)
            : base(entityType)
        {
            ContentTypeAttribute = EntityTypeInfo.GetCustomAttribute<ContentTypeAttribute>();

            if (ContentTypeAttribute == null)
            {
                throw Error.ArgumentOutOfRangeFormat(
                    "entityType",
                    SR.HarshEntityMetadataContentType_NoContentTypeAttribute,
                    entityType.FullName
                );
            }

            ContentTypeId = new ContentTypeIdBuilder(EntityTypeInfo).ToString();
        }

        public ContentTypeAttribute ContentTypeAttribute
        {
            get;
            private set;
        }

        public String ContentTypeId
        {
            get;
            private set;
        }
    }
}
