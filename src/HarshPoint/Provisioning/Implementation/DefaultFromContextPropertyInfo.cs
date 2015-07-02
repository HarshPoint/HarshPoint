using HarshPoint.Reflection;
using System;
using System.Reflection;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class DefaultFromContextPropertyInfo
    {
        private DefaultFromContextPropertyInfo(PropertyInfo property, DefaultFromContextAttribute attribute)
        {
            var propertyTypeInfo = property.PropertyType.GetTypeInfo();

            if (propertyTypeInfo.IsValueType)
            {
                throw Error.ProvisionerMetadataFormat(
                    SR.HarshProvisionerMetadata_NoValueTypeDefaultFromContext,
                    property.DeclaringType,
                    property.Name,
                    property.PropertyType
                );
            }

            ResolvedType = Resolvable.GetResolvedType(property.PropertyType);
            TagType = attribute.TagType;

            if (ResolvedType != null && TagType != null)
            {
                throw Error.InvalidOperation(
                    SR.HarshProvisionerMetadata_NoTagTypesOnResolvers,
                    property.DeclaringType,
                    property.Name
                );
            }
        }

        public Type ResolvedType
        {
            get;
            private set;
        }

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
