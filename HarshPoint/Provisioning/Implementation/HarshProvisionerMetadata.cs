using HarshPoint.Reflection;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
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
                GetPropertiesWith(typeof(DefaultFromContextAttribute), inherit: true)
                .Select(CreateDefaultFromContextProperty)
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

        private static DefaultFromContextPropertyInfo CreateDefaultFromContextProperty(PropertyInfo p)
        {
            if (p.PropertyType.IsConstructedGenericType)
            {
                var genericTypeDef = p.PropertyType.GetGenericTypeDefinition();

                if ((genericTypeDef == typeof(IResolve<>)) ||
                    (genericTypeDef == typeof(IResolveSingle<>)))
                {
                    var resolvedType = p.PropertyType.GenericTypeArguments[0];

                    return new DefaultFromContextPropertyInfo(p, resolvedType);
                }
            }

            throw Error.InvalidOperation(
                SR.HarshProvisionerMetadata_DefaultFromContextNotResolver,
                p.DeclaringType, 
                p.Name,
                p.PropertyType
            );
        }
        
        private static readonly TypeInfo HarshProvisionerBaseTypeInfo = typeof(HarshProvisionerBase).GetTypeInfo();

        public sealed class DefaultFromContextPropertyInfo
        {
            internal DefaultFromContextPropertyInfo(PropertyInfo p, Type resolved)
            {
                Property = p;
                ResolvedType = resolved;

                GetValue = p.MakeGetter<Object, Object>();
                SetValue = p.MakeSetter<Object, Object>();
            }

            public Func<Object, Object> GetValue
            {
                get;
                private set;
            }

            public PropertyInfo Property
            {
                get;
                private set;
            }

            public Type ResolvedType
            {
                get;
                private set;
            }

            public Action<Object, Object> SetValue
            {
                get;
                private set;
            }
            
        }
    }
}
