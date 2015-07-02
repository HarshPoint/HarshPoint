using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ParameterSetMetadataBuilder
    {
        private static readonly String DefaultDefaultParameterSetName = "__DefaultParameterSet";
        private static readonly StringComparer ParameterSetNameComparer = StringComparer.Ordinal;

        private readonly ILogger Logger;

        public ParameterSetMetadataBuilder(Type type)
        {
            if (type == null)
            {
                throw Error.ArgumentNull(nameof(type));
            }

            Logger = Log
                .ForContext<ParameterSetMetadataBuilder>()
                .ForContext("ProcessedType", type);

            ProcessedType = type;
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

        public IEnumerable<ParameterSetMetadata> Build()
        {
            Logger.Debug(
                "Building parameter set metadata for {ProcessedType}",
                ProcessedType
            );

            var parameters = BuildParameterMetadata();

            Logger.Debug("Found parameters: {@Parameters}", parameters);

            var commonParameters = parameters
                .Where(p => p.ParameterSetName == null)
                .ToArray();

            Logger.Debug("Found common parameters: {@CommonParameters}", commonParameters);

            var parameterSets = parameters
                .Where(p => p.ParameterSetName != null)
                .GroupBy(p => p.ParameterSetName, ParameterSetNameComparer)
                .Select(
                    (set, index) => new ParameterSetMetadata(
                        set.Key,
                        set.Concat(commonParameters),
                        IsDefaultParameterSet(set.Key, index)
                    )
                );

            if (parameterSets.Any())
            {
                Logger.Debug("Found parameter sets: {@ParameterSets}", parameterSets);

                return parameterSets;
            }

            var defaultParameterSet = new ParameterSetMetadata(
                DefaultDefaultParameterSetName,
                commonParameters,
                isDefault: true
            );

            Logger.Debug(
                "Found default parameter set: {@DefaultParameterSet}", 
                defaultParameterSet
            );

            return new[] { defaultParameterSet };
        }

        private Boolean IsDefaultParameterSet(String name, Int32 index)
        {
            if (DefaultParameterSetName != null)
            {
                return ParameterSetNameComparer.Equals(DefaultParameterSetName, name);
            }

            return (index == 0);
        }

        private IEnumerable<ParameterMetadata> BuildParameterMetadata()
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
                   let validation = property.GetCustomAttributes<ParameterValidationAttribute>(inherit: true)

                   from attr in paramAttrs
                   select new ParameterMetadata(
                       property,
                       attr,
                       defaultFromContext
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
                    throw Error.ProvisionerMetadataFormat(
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
                .GroupBy(p => p.ParameterSetName, ParameterSetNameComparer)
                .Where(set => set.Count() > 1)
                .Select(set => set.Key);

            if (nonUniqueParameterSetNames.Any())
            {
                throw Error.ProvisionerMetadataFormat(
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
                throw Error.ProvisionerMetadataFormat(
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
