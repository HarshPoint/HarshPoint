using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ParameterSetBuilder
    {
        private static readonly HarshLogger Logger = HarshLog.ForContext<ParameterSetBuilder>();

        public ParameterSetBuilder(Type type)
        {
            if (type == null)
            {
                throw Logger.Error.ArgumentNull(nameof(type));
            }

            ProcessedType = type;

            DefaultParameterSetName = type
                .GetTypeInfo()
                .GetCustomAttribute<DefaultParameterSetAttribute>(inherit: true)?
                .DefaultParameterSetName;
        }

        public String DefaultParameterSetName
        {
            get;
            set;
        }

        public Type ProcessedType
        {
            get;
            private set;
        }

        public IEnumerable<ParameterSet> Build()
        {
            Logger.Debug(
                "Building parameter set metadata for {ProcessedType}",
                ProcessedType
            );

            var parameters = BuildParameterMetadata();

            Logger.Debug(
                "{ProcessedType}: Default parameter set name: {DefaultParameterSetName}",
                ProcessedType,
                DefaultParameterSetName
            );

            Logger.Debug(
                "{ProcessedType}: All parameters: {@Parameters}", 
                ProcessedType,
                parameters
            );

            var commonParameters = parameters
                .Where(p => p.IsCommonParameter)
                .ToArray();

            Logger.Debug(
                "{ProcessedType}: Common parameters: {@CommonParameters}", 
                ProcessedType,
                commonParameters
            );

            var parameterSets = parameters
                .Where(p => !p.IsCommonParameter)
                .GroupBy(p => p.ParameterSetName, ParameterSet.NameComparer)
                .Select(
                    (set, index) => new ParameterSet(
                        set.Key,
                        set.Concat(commonParameters),
                        IsDefaultParameterSet(set.Key, index)
                    )
                );

            if ((DefaultParameterSetName != null) &&
                !parameterSets.Any(set => set.IsDefault))
            {
                throw Logger.Error.ProvisionerMetadata(
                    SR.HarshProvisionerMetadata_DefaultParameterSetNotFound,
                    DefaultParameterSetName,
                    ProcessedType
                );
            }

            if (parameterSets.Any())
            {
                Logger.Debug(
                    "{ProcessedType}: Parameter sets: {@ParameterSets}", 
                    ProcessedType,
                    parameterSets
                );

                return parameterSets;
            }

            var implicitParameterSet = new ParameterSet(
                ParameterSet.ImplicitParameterSetName,
                commonParameters,
                isDefault: true
            );

            Logger.Debug(
                "{ProcessedType}: Implicit parameter set: {@ImplicitParameterSet}", 
                ProcessedType,
                implicitParameterSet
            );

            return new[] { implicitParameterSet };
        }

        private Boolean IsDefaultParameterSet(String name, Int32 index)
        {
            if (DefaultParameterSetName != null)
            {
                return ParameterSet.NameComparer.Equals(
                    DefaultParameterSetName, 
                    name
                );
            }

            return (index == 0);
        }

        private IEnumerable<Parameter> BuildParameterMetadata()
        {
            return from property in ProcessedType.GetRuntimeProperties()

                   let paramAttrs = property
                      .GetCustomAttributes<ParameterAttribute>(inherit: true)
                      .ToArray()

                   where
                      paramAttrs.Any() &&
                      IsReadableAndWritable(property) &&
                      !HasNonUniqueParameterSetNames(property, paramAttrs) &&
                      !IsBothCommonParameterAndInParameterSet(property, paramAttrs)

                   let defaultFromContext = DefaultFromContextPropertyInfo.FromPropertyInfo(property)
                   let validationAttributes = property.GetCustomAttributes<ParameterValidationAttribute>(inherit: true)

                   from attr in paramAttrs
                   select new Parameter(
                       property,
                       attr,
                       defaultFromContext,
                       validationAttributes
                   );
        }

        private static Boolean IsBothCommonParameterAndInParameterSet(
            PropertyInfo property,
            IEnumerable<ParameterAttribute> attributes
        )
        {
            if (attributes.Any(a => a.ParameterSetName == null))
            {
                if (attributes.Count() > 1)
                {
                    throw Logger.Error.ProvisionerMetadata(
                        SR.HarshProvisionerMetadata_ParameterBothCommonAndInSet,
                        property.DeclaringType.FullName,
                        property.Name
                    );
                }
            }

            return false;
        }

        private static Boolean HasNonUniqueParameterSetNames(
            PropertyInfo property,
            IEnumerable<ParameterAttribute> attributes
        )
        {
            var nonUniqueParameterSetNames = attributes
                .GroupBy(p => p.ParameterSetName, ParameterSet.NameComparer)
                .Where(set => set.Count() > 1)
                .Select(set => set.Key);

            if (nonUniqueParameterSetNames.Any())
            {
                throw Logger.Error.ProvisionerMetadata(
                    SR.HarshProvisionerMetadata_MoreParametersWithSameSet,
                    property.DeclaringType.FullName,
                    property.Name,
                    String.Join(
                        ", ",
                        nonUniqueParameterSetNames.Select(
                            set => '"' + set + '"'
                        )
                    )
                );
            }

            return false;
        }

        private static Boolean IsReadableAndWritable(PropertyInfo property)
        {
            if (!property.CanRead || 
                !property.CanWrite ||
                !IsPublicInstance(property.GetMethod) ||
                !IsPublicInstance(property.SetMethod)
            )
            {
                throw Logger.Error.ProvisionerMetadata(
                    SR.HarshProvisionerMetadata_ParameterMustBeReadWriteInstance,
                    property.DeclaringType.FullName,
                    property.Name
                );
            }

            return true;
        }

        private static Boolean IsPublicInstance(MethodBase method)
        {
            return method.IsPublic && !method.IsStatic;
        }
    }
}
