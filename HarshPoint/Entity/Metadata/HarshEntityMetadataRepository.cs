using System;
using System.Reflection;

namespace HarshPoint.Entity.Metadata
{
    internal sealed class HarshEntityMetadataRepository
    {
        private static readonly HarshEntityMetadataRepository _singleton =
            new HarshEntityMetadataRepository();

        private readonly HarshInMemoryObjectCache<TypeInfo, HarshEntityMetadata> _cache =
            new HarshInMemoryObjectCache<TypeInfo, HarshEntityMetadata>();

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

            return _cache.GetOrAdd(typeInfo, ti => HarshEntityMetadata.Create(this, ti));
        }

        public static HarshEntityMetadataRepository Current
        {
            get { return _singleton; }
        }
    }
}
