using System;
using System.Reflection;

namespace HarshPoint.Entity.Metadata
{
    internal sealed class HarshEntityMetadataContentType : HarshEntityMetadata
    {
        internal HarshEntityMetadataContentType(Type entityType)
            : this(entityType.GetTypeInfo())
        {
        }

        internal HarshEntityMetadataContentType(TypeInfo entityTypeInfo)
            : base(entityTypeInfo)
        {
            ContentTypeAttribute = EntityTypeInfo.GetCustomAttribute<ContentTypeAttribute>();

            if (ContentTypeAttribute == null)
            {
                throw Error.ArgumentOutOfRangeFormat(
                    "entityType",
                    SR.HarshEntityMetadataContentType_NoContentTypeAttribute,
                    entityTypeInfo.FullName
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
