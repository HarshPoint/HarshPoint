using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;

namespace HarshPoint.Entity.Metadata
{
    public abstract class HarshEntityMetadata : HarshObjectMetadata
    {
        internal HarshEntityMetadata(HarshEntityMetadataRepository repository, TypeInfo entityTypeInfo)
            : base(entityTypeInfo)
        {
            if (repository == null)
            {
                throw Error.ArgumentNull(nameof(repository));
            }

            if (!HarshEntityTypeInfo.IsAssignableFrom(ObjectTypeInfo))
            {
                throw Error.ArgumentOutOfRange_TypeNotAssignableFrom(
                    nameof(entityTypeInfo),
                    HarshEntityTypeInfo,
                    ObjectTypeInfo
                );
            }

            Repository = repository;

            GetBaseEntity();
            CreateDeclaredFields();
        }

        public HarshEntityMetadata BaseEntity
        {
            get;
            private set;
        }

        public IReadOnlyCollection<HarshFieldMetadata> DeclaredFields
        {
            get;
            private set;
        }

        internal HarshEntityMetadataRepository Repository
        {
            get;
            private set;
        }

        private void GetBaseEntity()
        {
            if (ObjectTypeInfo.BaseType == typeof(HarshEntity))
            {
                return;
            }

            BaseEntity = Repository.GetMetadata(ObjectTypeInfo.BaseType);
        }

        private void CreateDeclaredFields()
        {
            var declared = ImmutableList.CreateBuilder<HarshFieldMetadata>();

            foreach (var property in ObjectTypeInfo.DeclaredProperties)
            {
                if (!property.CanWrite || !property.SetMethod.IsPublic)
                {
                    Trace.WriteInfo(
                        "CreateFieldMetadata: skipping non-writable property {0}.{1}.",
                        ObjectTypeInfo.FullName,
                        property.Name
                    );
                    continue;
                }

                if (!property.CanRead || !property.GetMethod.IsPublic)
                {
                    Trace.WriteInfo(
                        "CreateFieldMetadata: skipping non-readable property {0}.{1}.",
                        ObjectTypeInfo.FullName,
                        property.Name
                    );
                    continue;
                }

                var fieldAttr = property.GetCustomAttribute<FieldAttribute>(inherit: false);

                if (fieldAttr == null)
                {
                    Trace.WriteInfo(
                        "CreateFieldMetadata: skipping property {0}.{1}, has no FieldAttribute.",
                        ObjectTypeInfo.FullName,
                        property.Name
                    );
                    continue;
                }

                declared.Add(new HarshFieldMetadata(property, fieldAttr));
            }

            DeclaredFields = declared.ToImmutable();
        }

        internal static HarshEntityMetadata Create(HarshEntityMetadataRepository repository, TypeInfo typeInfo)
        {
            if (repository== null)
            {
                throw Error.ArgumentNull(nameof(repository));
            }

            if (typeInfo == null)
            {
                throw Error.ArgumentNull(nameof(typeInfo));
            }

            if (typeInfo.IsDefined(typeof(ContentTypeAttribute), inherit: false))
            {
                return new HarshEntityMetadataContentType(repository, typeInfo);
            }

            throw new NotImplementedException("TODO: more entity types to come :)");
        }

        private static readonly HarshTraceSource Trace =
            new HarshTraceSource(typeof(HarshEntityMetadata));

        private static readonly TypeInfo HarshEntityTypeInfo = typeof(HarshEntity).GetTypeInfo();
    }
}
