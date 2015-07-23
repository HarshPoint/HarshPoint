using HarshPoint.ObjectModel;
using HarshPoint.Reflection;
using System;
using System.Reflection;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class DefaultFromContextProperty
    {
        public DefaultFromContextProperty(
            PropertyAccessor accessor,
            DefaultFromContextAttribute attribute
        )
        {
            if (accessor == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(accessor));
            }

            if (attribute == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(attribute));
            }

            Accessor = accessor;
            Attribute = attribute;
            ResolvedType = Resolvable.GetResolvedType(PropertyTypeInfo);

            ValidateIsNullable();
            ValidateTagTypeIsIDefaultFromContextTag();
            ValidateWithTagNotResolvable();
        }

        public PropertyAccessor Accessor { get; private set; }
        public DefaultFromContextAttribute Attribute { get; private set; }
        public String Name => PropertyInfo.Name;
        public PropertyInfo PropertyInfo => Accessor.PropertyInfo;
        public Type PropertyType => PropertyInfo.PropertyType;
        public TypeInfo PropertyTypeInfo => PropertyType.GetTypeInfo();
        public Type ResolvedType { get; private set; }
        public Type TagType => Attribute.TagType;

        private void ValidateIsNullable()
        {
            if (!PropertyTypeInfo.IsNullable())
            {
                throw Logger.Fatal.ObjectMetadata(
                    SR.HarshProvisionerMetadata_NoValueTypeDefaultFromContext,
                    PropertyInfo.DeclaringType,
                    PropertyInfo.Name,
                    PropertyInfo.PropertyType
                );
            }
        }

        private void ValidateWithTagNotResolvable()
        {
            if ((TagType != null) && (ResolvedType != null))
            {
                throw Logger.Fatal.ObjectMetadata(
                    SR.HarshProvisionerMetadata_NoTagTypesOnResolvers,
                    PropertyInfo.DeclaringType,
                    PropertyInfo.Name
                );
            }
        }

        private void ValidateTagTypeIsIDefaultFromContextTag()
        {
            if (TagType == null)
            {
                return;
            }

            if (!IDefaultFromContextTagTypeInfo.IsAssignableFrom(TagType.GetTypeInfo()))
            {
                throw Logger.Fatal.ObjectMetadata(
                    SR.HarshProvisionerMetadata_TagTypeNotAssignableFromIDefaultFromContextTag,
                    TagType,
                    PropertyInfo.DeclaringType,
                    PropertyInfo.Name
                );
            }
        }

        private static readonly TypeInfo IDefaultFromContextTagTypeInfo
            = typeof(IDefaultFromContextTag).GetTypeInfo();

        private static readonly HarshLogger Logger = HarshLog.ForContext<DefaultFromContextProperty>();
    }
}
