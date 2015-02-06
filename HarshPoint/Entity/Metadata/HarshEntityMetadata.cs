using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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

            DeclaredFields = CreateFieldMetadata().ToImmutableList();
        }

        public Type EntityType
        {
            get;
            private set;
        }

        public IReadOnlyCollection<HarshFieldMetadata> DeclaredFields
        {
            get;
            private set;
        }

        internal TypeInfo EntityTypeInfo
        {
            get;
            private set;
        }

        private IEnumerable<HarshFieldMetadata> CreateFieldMetadata()
        {
            foreach (var property in EntityTypeInfo.DeclaredProperties)
            {
                if (!property.CanWrite)
                {
                    Trace.WriteLine(
                        "CreateFieldMetadata: skipping non-writable property {0}.{1}.",
                        EntityTypeInfo.FullName,
                        property.Name
                    );
                    continue;
                }

                if (!property.CanRead)
                {
                    Trace.WriteLine(
                        "CreateFieldMetadata: skipping non-readable property {0}.{1}.",
                        EntityTypeInfo.FullName,
                        property.Name
                    );
                    continue;
                }

                var fieldAttr = property.GetCustomAttribute<FieldAttribute>();

                if (fieldAttr == null)
                {
                    Trace.WriteLine(
                        "CreateFieldMetadata: skipping property {0}.{1}, has no FieldAttribute.",
                        EntityTypeInfo.FullName,
                        property.Name
                    );
                    continue;
                }

                yield return new HarshFieldMetadata(property, fieldAttr);
            }
        }

        internal static readonly TypeInfo HarshEntityTypeInfo = 
            typeof(HarshEntity).GetTypeInfo();

        private static readonly HarshTraceSource Trace = 
            new HarshTraceSource(typeof(HarshEntityMetadata));
    }
}
