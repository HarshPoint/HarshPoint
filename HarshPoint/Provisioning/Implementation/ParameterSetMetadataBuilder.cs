using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ParameterSetMetadataBuilder
    {
        private const String DefaultDefaultParameterSetName = "__DefaultParameterSet";

        private static readonly ILogger Logger = Log.ForContext<ParameterSetMetadataBuilder>();
        private static readonly StringComparer ParameterSetNameComparer = StringComparer.Ordinal;

        public ParameterSetMetadataBuilder(IEnumerable<PropertyInfo> properties)
        {
            if (properties == null)
            {
                throw Error.ArgumentNull(nameof(properties));
            }

            Properties = properties;
        }

        public String DefaultParameterSetName
        {
            get;
            set;
        }

        public IEnumerable<PropertyInfo> Properties
        {
            get;
            private set;
        }

        public IEnumerable<ParameterSetMetadata> Build()
        {
            var parameters = BuildParameterMetadata();

            var commonParameters = parameters
                .Where(p => p.ParameterSetName == null)
                .ToArray();

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
                return parameterSets;

            }

            return new[]
            {
                new ParameterSetMetadata(
                    DefaultDefaultParameterSetName,
                    commonParameters,
                    isDefault: true
                )
            };
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
            return from property in Properties

                   let attrs = property
                      .GetCustomAttributes<ParameterAttribute>(inherit: true)
                      .ToArray()

                   where
                      attrs.Any() &&
                      IsReadableAndWritable(property) &&
                      !HasNonUniqueParameterSetNames(property, attrs) &&
                      !IsBothCommonParameterAndInParameterSet(property, attrs)

                   from attr in attrs
                   select new ParameterMetadata(property, attr);
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
                        SR.HarshProvisionerMetadataException_ParameterBothCommonAndInSet,
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
                    SR.HarshProvisionerMetadataException_MoreParametersWithSameSet,
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
                !property.GetMethod.IsPublic || 
                !property.SetMethod.IsPublic)
            {
                throw Error.ProvisionerMetadataFormat(
                    SR.HarshProvisionerMetadataException_ParameterMustBeReadWrite,
                    property.DeclaringType.FullName,
                    property.Name
                );
            }

            return true;
        }
    }
}
