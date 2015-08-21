using HarshPoint.ObjectModel;
using HarshPoint.Reflection;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace HarshPoint.Provisioning.Implementation
{
    public sealed class HarshProvisionerMetadata : HarshParametrizedObjectMetadata
    {
        internal HarshProvisionerMetadata(Type type)
            : base(type, ProvisioningDefaultValuePolicy.Instance)
        {
            if (!HarshProvisionerBaseTypeInfo.IsAssignableFrom(ObjectTypeInfo))
            {
                throw Logger.Fatal.ArgumentTypeNotAssignableTo(
                    nameof(type),
                    type,
                    HarshProvisionerBaseTypeInfo.AsType()
                );
            }

            ContextType = ObjectType.GetRuntimeBaseTypeChain()
                .FirstOrDefault(t =>
                    t.IsGenericType &&
                    t.GetGenericTypeDefinition() == typeof(HarshProvisionerBase<>)
                )?
                .GenericTypeArguments
                .First();

            DefaultFromContextPropertyBinder = new DefaultFromContextPropertyBinder(
                ReadableWritableInstancePropertiesWithSingle<DefaultFromContextAttribute>(inherit: true)
                .Select(t => new DefaultFromContextProperty(t.Item1, t.Item2))
            );

            ParametersMandatoryWhenCreating =
                Parameters
                .Where(p => p.IsDefined(typeof(MandatoryWhenCreatingAttribute), inherit: true))
                .ToImmutableHashSet();

            ResolvedPropertyBinder = new ResolvedPropertyBinder(
                ReadableWritableInstanceProperties
                .Where(ResolvedPropertyTypeInfo.IsResolveType)
            );

            UnprovisionDeletesUserData = GetDeletesUserData("OnUnprovisioningAsync");
        }

        public Type ContextType { get; }

        public Boolean UnprovisionDeletesUserData { get; }

        internal DefaultFromContextPropertyBinder DefaultFromContextPropertyBinder { get; }

        internal ResolvedPropertyBinder ResolvedPropertyBinder { get; }

        public Boolean IsMandatoryWhenCreating(Parameter parameter)
        {
            if (parameter == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(parameter));
            }

            return ParametersMandatoryWhenCreating.Contains(parameter);
        }

        private IEnumerable<Parameter> ParametersMandatoryWhenCreating { get; }

        private Boolean GetDeletesUserData(String methodName)
        {
            var method = ObjectType
                .GetRuntimeMethods()
                .Single(m =>
                    m.IsFamily &&
                    !m.IsStatic &&
                    StringComparer.Ordinal.Equals(m.Name, methodName) &&
                    !m.GetParameters().Any()
                );

            var deletesUserData = method
                .GetRuntimeBaseMethodChain()
                .Any(
                    m => !m.IsDefined(
                        typeof(NeverDeletesUserDataAttribute),
                        inherit: false
                    )
                );

            Logger.Debug(
                "{ObjectType}: {Method} DeletesUserData = {DeletesUserData}",
                ObjectType,
                methodName,
                deletesUserData
            );

            return deletesUserData;
        }

        private static readonly TypeInfo HarshProvisionerBaseTypeInfo
            = typeof(HarshProvisionerBase).GetTypeInfo();

        private static readonly HarshLogger Logger
            = HarshLog.ForContext<HarshProvisionerMetadata>();
    }
}
