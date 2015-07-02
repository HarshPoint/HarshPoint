using HarshPoint.Reflection;
using System;
using System.Reflection;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class DefaultFromContextPropertyInfo
    {
        internal DefaultFromContextPropertyInfo(PropertyInfo property, DefaultFromContextAttribute attribute)
        {
            Property = property;

            if (PropertyTypeInfo.IsValueType)
            {
                throw Error.InvalidOperation(
                    SR.HarshProvisionerMetadata_NoValueTypeDefaultFromContext,
                    property.DeclaringType,
                    property.Name,
                    PropertyType
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

            Getter = property.MakeGetter<Object, Object>();
            Setter = property.MakeSetter<Object, Object>();
        }

        public Boolean HasDefaultValue(Object provisioner)
        {
            var value = Getter(provisioner);

            if (value == null)
            {
                return true;
            }

            var str = value as String;

            if ((str != null) && (str.Length == 0))
            {
                return true;
            }

            return false;
        }

        public Func<Object, Object> Getter
        {
            get;
            private set;
        }

        public PropertyInfo Property
        {
            get;
            private set;
        }

        public Type PropertyType => Property.PropertyType;

        public TypeInfo PropertyTypeInfo => PropertyType.GetTypeInfo();

        public Type ResolvedType
        {
            get;
            private set;
        }

        public Action<Object, Object> Setter
        {
            get;
            private set;
        }

        public Type TagType
        {
            get;
            private set;
        }
    }
}
