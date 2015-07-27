using HarshPoint.ObjectModel;
using HarshPoint.Reflection;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using HarshPoint.Reflection;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class HarshProvisionerMetadata : HarshObjectMetadata
    {
        internal HarshProvisionerMetadata(Type type)
            : base(type)
        {
            if (!HarshProvisionerBaseTypeInfo.IsAssignableFrom(ObjectTypeInfo))
            {
                throw Logger.Fatal.ArgumentTypeNotAssignableTo(
                    nameof(type),
                    type,
                    HarshProvisionerBaseTypeInfo.AsType()
                );
            }

            ParameterSets = new ParameterSetBuilder(this, ProvisioningDefaultValuePolicy.Instance)
                .Build()
                .ToImmutableArray();

            ContextType = ObjectType.GetRuntimeBaseTypeChain()
                .FirstOrDefault(t =>
                    t.IsGenericType &&
                    t.GetGenericTypeDefinition() == typeof(HarshProvisionerBase<>)
                )?
                .GenericTypeArguments
                .First();

            ParameterProperties = Parameters
                .Select(p => p.PropertyAccessor)
                .Distinct()
                .ToImmutableArray();

            DefaultParameterSet = ParameterSets.Single(set => set.IsDefault);

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
        public Type ContextType
        {
            get;
            private set;
        }

        public DefaultFromContextPropertyBinder DefaultFromContextPropertyBinder { get; }

        public ParameterSet DefaultParameterSet { get; }

        public IEnumerable<Parameter> Parameters
            => ParameterSets.SelectMany(set => set.Parameters);

        public IEnumerable<ParameterSet> ParameterSets { get; }

        public IEnumerable<PropertyAccessor> ParameterProperties { get; }

        public ResolvedPropertyBinder ResolvedPropertyBinder { get; }

        public Boolean UnprovisionDeletesUserData { get; }

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
