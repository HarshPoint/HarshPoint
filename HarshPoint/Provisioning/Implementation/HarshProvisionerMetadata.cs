using HarshPoint.Reflection;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class HarshProvisionerMetadata : HarshObjectMetadata
    {
        public HarshProvisionerMetadata(Type type)
            : base(type)
        {
            if (!HarshProvisionerBaseTypeInfo.IsAssignableFrom(ObjectTypeInfo))
            {
                throw Error.ArgumentOutOfRange_TypeNotAssignableFrom(
                    nameof(type),
                    HarshProvisionerBaseTypeInfo,
                    ObjectTypeInfo
                );
            }

            DefaultFromContextProperties =
                GetPropertiesWith<DefaultFromContextAttribute>(inherit: true)
                .Select(tuple => new DefaultFromContextPropertyInfo(tuple.Item1, tuple.Item2))
                .ToImmutableHashSet();

            UnprovisionDeletesUserData = GetDeletesUserData("OnUnprovisioningAsync");
        }

        public IImmutableSet<DefaultFromContextPropertyInfo> DefaultFromContextProperties
        {
            get;
            private set;
        }

        public Boolean UnprovisionDeletesUserData
        {
            get;
            private set;
        }

        private Boolean GetDeletesUserData(String methodName)
        {
            var method = ObjectType
                .GetRuntimeMethods()
                .Instance()
                .NonPublic()
                .Single(m =>
                    StringComparer.Ordinal.Equals(m.Name, methodName) &&
                    !m.GetParameters().Any()
                );

            return method
                .GetRuntimeBaseMethodChain()
                .Any(
                    m => !m.IsDefined(
                        typeof(NeverDeletesUserDataAttribute),
                        inherit: false
                    )
                );
        }

        private static readonly TypeInfo HarshProvisionerBaseTypeInfo = typeof(HarshProvisionerBase).GetTypeInfo();

        public sealed class DefaultFromContextPropertyInfo
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
}
