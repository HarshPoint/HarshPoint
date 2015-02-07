using System;
using System.Collections.Immutable;
using System.Reflection;

namespace HarshPoint.Entity.Metadata
{
    internal sealed class HarshEntityMetadataRepository
    {
        private ImmutableDictionary<TypeInfo, HarshEntityMetadata> _repo;

        private HarshEntityMetadataRepository()
        {
        }

        public HarshEntityMetadata GetMetadata(Type type)
        {
            if (type == null)
            {
                throw Error.ArgumentNull("type");
            }

            return GetMetadata(type.GetTypeInfo());
        }

        public HarshEntityMetadata GetMetadata(TypeInfo typeInfo)
        {
            if (typeInfo == null)
            {
                throw Error.ArgumentNull("typeInfo");
            }

            HarshEntityMetadata result;

            if (!_repo.TryGetValue(typeInfo, out result))
            {
                result = HarshEntityMetadata.Create(typeInfo);
                result.Repository = this;

                _repo = _repo.Add(typeInfo, result);
            }

            return result;
        }

        public static HarshEntityMetadataRepository Current
        {
            get { return _singleton; }
        }

        private static readonly HarshEntityMetadataRepository _singleton =
            new HarshEntityMetadataRepository();
    }
}
