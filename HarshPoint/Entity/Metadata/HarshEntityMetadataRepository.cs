using System;
using System.Collections.Immutable;
using System.Reflection;

namespace HarshPoint.Entity.Metadata
{
    internal sealed class HarshEntityMetadataRepository
    {
        private static readonly HarshEntityMetadataRepository _singleton =
            new HarshEntityMetadataRepository();

        private ImmutableDictionary<TypeInfo, HarshEntityMetadata> _cache =
            ImmutableDictionary<TypeInfo, HarshEntityMetadata>.Empty;

        private HarshEntityMetadataRepository()
        {
        }

        public HarshEntityMetadata GetMetadata(Type type)
        {
            if (type == null)
            {
                throw Error.ArgumentNull(nameof(type));
            }

            return GetMetadata(type.GetTypeInfo());
        }

        public HarshEntityMetadata GetMetadata(TypeInfo typeInfo)
        {
            if (typeInfo == null)
            {
                throw Error.ArgumentNull(nameof(typeInfo));
            }

            return ImmutableInterlocked.GetOrAdd(
                ref _cache,
                typeInfo,
                ti => HarshEntityMetadata.Create(this, ti)
            );
        }

        public static HarshEntityMetadataRepository Current
        {
            get { return _singleton; }
        }
    }
}
