using HarshPoint.Reflection;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using HarshPoint.ObjectModel;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class HarshProvisionerMetadata : HarshObjectMetadata
    {
        public HarshProvisionerMetadata(Type type)
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

            ParameterSets = new ParameterSetBuilder(ObjectType)
                .Build()
                .ToImmutableArray();

            DefaultParameterSet = ParameterSets.Single(set => set.IsDefault);

            DefaultFromContextPropertyBinder = new DefaultFromContextPropertyBinder(
                ReadableWritableInstancePropertiesWith<DefaultFromContextAttribute>(inherit: true).Select(
                    t => new DefaultFromContextProperty(t.Item1, t.Item2)
                )
            );

            ResolvedPropertyBinder = new ResolvedPropertyBinder(
                ReadableWritableInstanceProperties.Where(
                    p => ResolvedPropertyTypeInfo.IsResolveType(p.PropertyTypeInfo)
                )
            );

            UnprovisionDeletesUserData = GetDeletesUserData("OnUnprovisioningAsync");
        }

        public DefaultFromContextPropertyBinder DefaultFromContextPropertyBinder
        {
            get;
            private set;
        }

        public ParameterSet DefaultParameterSet
        {
            get;
            private set;
        }

        public IEnumerable<Parameter> Parameters
            => ParameterSets.SelectMany(set => set.Parameters);

        public IReadOnlyList<ParameterSet> ParameterSets
        {
            get;
            private set;
        }

        public ResolvedPropertyBinder ResolvedPropertyBinder
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

        private static readonly TypeInfo HarshProvisionerBaseTypeInfo = typeof(HarshProvisionerBase).GetTypeInfo();
        private static readonly HarshLogger Logger = HarshLog.ForContext<HarshProvisionerMetadata>();
    }
}
