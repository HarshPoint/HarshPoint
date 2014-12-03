using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace HarshPoint.Entity.Metadata
{
    public abstract class HarshEntityMetadata
    {
        internal HarshEntityMetadata(Type entityType)
        {
            if (entityType == null)
            {
                throw Error.ArgumentNull("entityType");
            }

            EntityType = entityType;
            EntityTypeInfo = entityType.GetTypeInfo();

            if (!EntityTypeInfo.IsSubclassOf(typeof(HarshEntity)))
            {
                throw Error.ArgumentOutOfRangeFormat(
                    "entityType",
                    SR.HarshEntityMetadata_TypeNotAnEntity,
                    entityType.FullName
                );
            }

            Fields = CreateFieldMetadata();
        }

        public Type EntityType
        {
            get;
            private set;
        }

        public IReadOnlyCollection<HarshFieldMetadata> Fields
        {
            get;
            private set;
        }

        internal TypeInfo EntityTypeInfo
        {
            get;
            private set;
        }

        private ImmutableList<HarshFieldMetadata> CreateFieldMetadata()
        {
            return (from property in EntityTypeInfo.DeclaredProperties
                    let fieldAttr = property.GetCustomAttribute<FieldAttribute>()
                    where fieldAttr != null
                    select new HarshFieldMetadata(property, fieldAttr))
                   .ToImmutableList();
        }

        internal static readonly TypeInfo HarshEntityTypeInfo = typeof(HarshEntity).GetTypeInfo();
    }
}
