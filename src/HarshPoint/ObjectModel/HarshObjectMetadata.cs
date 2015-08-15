using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace HarshPoint.ObjectModel
{
    public class HarshObjectMetadata
    {
        public HarshObjectMetadata(Type type)
        {
            if (type == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(type));
            }

            ObjectType = type;
            ObjectTypeInfo = type.GetTypeInfo();

            ReadableWritableInstanceProperties =
                InitReadableWritableInstanceProperties();
        }

        public HarshObjectMetadata(TypeInfo typeInfo)
        {
            if (typeInfo == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(typeInfo));
            }

            ObjectType = typeInfo.AsType();
            ObjectTypeInfo = typeInfo;

            ReadableWritableInstanceProperties =
                InitReadableWritableInstanceProperties();
        }

        public Type ObjectType
        {
            get;

        }

        public TypeInfo ObjectTypeInfo
        {
            get;
        }

        public IEnumerable<PropertyAccessor> ReadableWritableInstanceProperties
        {
            get;
        }

        public IEnumerable<IGrouping<PropertyAccessor, TAttribute>> ReadableWritableInstancePropertiesWith<TAttribute>(Boolean inherit)
            where TAttribute : Attribute
            => ReadableWritableInstanceProperties
                .Select(p => HarshGrouping.Create(p, p.PropertyInfo.GetCustomAttributes<TAttribute>(inherit).ToArray()))
                .Where(g => g.Any())
                .ToArray();

        public IEnumerable<Tuple<PropertyAccessor, TAttribute>> ReadableWritableInstancePropertiesWithSingle<TAttribute>(Boolean inherit)
            where TAttribute : Attribute
            => ReadableWritableInstanceProperties
                .Select(p => Tuple.Create(p, p.PropertyInfo.GetCustomAttribute<TAttribute>(inherit)))
                .Where(t => t.Item2 != null)
                .ToArray();

        private IEnumerable<PropertyAccessor> InitReadableWritableInstanceProperties()
            => ObjectType
                .GetRuntimeProperties()
                .Where(p => p.CanRead && p.CanWrite && !p.GetMethod.IsStatic && !p.SetMethod.IsStatic)
                .Select(p => new PropertyAccessor(p))
                .ToImmutableArray();

        private static readonly HarshLogger Logger = HarshLog.ForContext<HarshObjectMetadata>();
    }
}
