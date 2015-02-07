using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
            get { return Lazy.Value; }
        }

        private static readonly Lazy<HarshEntityMetadataRepository> Lazy =
            new Lazy<HarshEntityMetadataRepository>(LazyThreadSafetyMode.ExecutionAndPublication);
    }
}
