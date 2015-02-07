using HarshPoint.Provisioning;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;

namespace HarshPoint.Entity.Metadata
{
    public abstract class HarshEntityMetadata
    {
        private HarshEntityMetadata _baseEntity;

        internal HarshEntityMetadata(TypeInfo entityTypeInfo)
        {
            if (entityTypeInfo == null)
            {
                throw Error.ArgumentNull("entityType");
            }

            EntityTypeInfo = entityTypeInfo;

            if (!EntityTypeInfo.IsSubclassOf(typeof(HarshEntity)))
            {
                throw Error.ArgumentOutOfRangeFormat(
                    "entityType",
                    SR.HarshEntityMetadata_TypeNotAnEntity,
                    entityTypeInfo.FullName
                );
            }

            CreateFieldMetadata();
        }

        public IReadOnlyCollection<HarshFieldMetadata> DeclaredFields
        {
            get;
            private set;
        }

        public TypeInfo EntityTypeInfo
        {
            get;
            private set;
        }

        public HarshEntityMetadata BaseEntity
        {
            get
            {
                if (EntityTypeInfo.BaseType == typeof(HarshEntity))
                {
                    return null;
                }

                if (_baseEntity == null)
                {
                    ValidateRepository();
                    _baseEntity = Repository.GetMetadata(EntityTypeInfo.BaseType);
                }

                return _baseEntity;
            }
        }

        internal HarshEntityMetadataRepository Repository
        {
            get;
            set;
        }

        private void CreateFieldMetadata()
        {
            var declared = ImmutableList.CreateBuilder<HarshFieldMetadata>();

            foreach (var property in EntityTypeInfo.DeclaredProperties)
            {
                if (!property.CanWrite || !property.SetMethod.IsPublic)
                {
                    Trace.WriteLine(
                        "CreateFieldMetadata: skipping non-writable property {0}.{1}.",
                        EntityTypeInfo.FullName,
                        property.Name
                    );
                    continue;
                }

                if (!property.CanRead || !property.GetMethod.IsPublic)
                {
                    Trace.WriteLine(
                        "CreateFieldMetadata: skipping non-readable property {0}.{1}.",
                        EntityTypeInfo.FullName,
                        property.Name
                    );
                    continue;
                }

                var fieldAttr = property.GetCustomAttribute<FieldAttribute>(inherit: false);

                if (fieldAttr == null)
                {
                    Trace.WriteLine(
                        "CreateFieldMetadata: skipping property {0}.{1}, has no FieldAttribute.",
                        EntityTypeInfo.FullName,
                        property.Name
                    );
                    continue;
                }

                declared.Add(new HarshFieldMetadata(property, fieldAttr));
            }

            DeclaredFields = declared.ToImmutable();
        }

        private void ValidateRepository()
        {
            if (Repository == null)
            {
                throw new InvalidOperationException(SR.HarshEntityMetadata_DoesntBelongToARepo);
            }
        }

        internal static HarshEntityMetadata Create(TypeInfo typeInfo)
        {
            if (typeInfo == null)
            {
                throw Error.ArgumentNull("typeInfo");
            }

            if (typeInfo.IsDefined(typeof(ContentTypeAttribute), inherit: false))
            {
                return new HarshEntityMetadataContentType(typeInfo);
            }

            throw new NotImplementedException("TODO: more entity types to come :)");
        }

        private static readonly HarshTraceSource Trace =
            new HarshTraceSource(typeof(HarshEntityMetadata));
    }
}
