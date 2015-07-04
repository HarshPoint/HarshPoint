﻿using Destructurama.Attributed;
using System;
using System.Reflection;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class DefaultFromContextPropertyInfo
    {
        private static readonly TypeInfo IDefaultFromContextTagTypeInfo =
            typeof(IDefaultFromContextTag).GetTypeInfo();

        private static readonly HarshLogger Logger =
            HarshLog.ForContext<DefaultFromContextPropertyInfo>();

        private DefaultFromContextPropertyInfo(PropertyInfo property, DefaultFromContextAttribute attribute)
        {
            var propertyTypeInfo = property.PropertyType.GetTypeInfo();

            if (propertyTypeInfo.IsValueType)
            {
                throw Logger.Error.ProvisionerMetadata(
                    SR.HarshProvisionerMetadata_NoValueTypeDefaultFromContext,
                    property.DeclaringType,
                    property.Name,
                    property.PropertyType
                );
            }

            ResolvedType = Resolvable.GetResolvedType(property.PropertyType);
            TagType = attribute.TagType;

            if (TagType != null)
            {
                if (ResolvedType != null)
                {
                    throw Logger.Error.ProvisionerMetadata(
                        SR.HarshProvisionerMetadata_NoTagTypesOnResolvers,
                        property.DeclaringType,
                        property.Name
                    );
                }

                if (!IDefaultFromContextTagTypeInfo.IsAssignableFrom(TagType.GetTypeInfo()))
                {
                    throw Logger.Error.ProvisionerMetadata(
                        SR.HarshProvisionerMetadata_TagTypeNotAssignableFromIDefaultFromContextTag,
                        TagType,
                        property.DeclaringType,
                        property.Name
                    );
                }
            }
        }

        [LogAsScalar]
        public Type ResolvedType
        {
            get;
            private set;
        }

        [LogAsScalar]
        public Type TagType
        {
            get;
            private set;
        }

        public static DefaultFromContextPropertyInfo FromPropertyInfo(PropertyInfo property)
        {
            var attr = property.GetCustomAttribute<DefaultFromContextAttribute>(inherit: true);

            if (attr != null)
            {
                return new DefaultFromContextPropertyInfo(property, attr);
            }

            return null;
        }
    }
}
