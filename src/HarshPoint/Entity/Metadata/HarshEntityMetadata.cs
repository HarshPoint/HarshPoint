using HarshPoint.ObjectModel;
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
                throw Logger.Fatal.ArgumentNull(nameof(repository));
            }

            if (!HarshEntityTypeInfo.IsAssignableFrom(ObjectTypeInfo))
            {
                throw Logger.Fatal.ArgumentTypeNotAssignableTo(
                    nameof(entityTypeInfo),
                    entityTypeInfo.AsType(),
                    HarshEntityTypeInfo.AsType()
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
                    Logger.Debug(
                        "CreateFieldMetadata: skipping non-writable property {Type}.{Property}.",
                        ObjectTypeInfo,
                        property.Name
                    );
                    continue;
                }

                if (!property.CanRead || !property.GetMethod.IsPublic)
                {
                    Logger.Debug(
                        "CreateFieldMetadata: skipping non-readable property {Type}.{Property}.",
                        ObjectTypeInfo,
                        property.Name
                    );
                    continue;
                }

                var fieldAttr = property.GetCustomAttribute<FieldAttribute>(inherit: false);

                if (fieldAttr == null)
                {
                    Logger.Debug(
                        "CreateFieldMetadata: skipping property {Type}.{Property}, has no FieldAttribute.",
                        ObjectTypeInfo,
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
            if (repository == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(repository));
            }

            if (typeInfo == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(typeInfo));
            }

            if (typeInfo.IsDefined(typeof(ContentTypeAttribute), inherit: false))
            {
                return new HarshEntityMetadataContentType(repository, typeInfo);
            }

            throw new NotImplementedException("TODO: more entity types to come :)");
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<HarshEntityMetadata>();
        private static readonly TypeInfo HarshEntityTypeInfo = typeof(HarshEntity).GetTypeInfo();
    }
}
