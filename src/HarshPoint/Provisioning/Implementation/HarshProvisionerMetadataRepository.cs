using System;
using System.Collections.Immutable;

namespace HarshPoint.Provisioning.Implementation
{
    internal static class HarshProvisionerMetadataRepository
    {
        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(HarshProvisionerMetadataRepository));

        private static ImmutableDictionary<Type, HarshProvisionerMetadata> _dict
            = ImmutableDictionary<Type, HarshProvisionerMetadata>.Empty;

        private static HarshProvisionerMetadata Create(Type t)
            => new HarshProvisionerMetadata(t);

        public static HarshProvisionerMetadata Get(Type type)
        {
            if (type == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(type));
            }

            return ImmutableInterlocked.GetOrAdd(ref _dict, type, Create);
        }
    }
}
