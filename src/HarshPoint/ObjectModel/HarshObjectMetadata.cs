using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
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

            InitReadableWritableInstanceProperties();
        }

        public Type ObjectType
        {
            get;
            private set;
        }

        public TypeInfo ObjectTypeInfo
        {
            get;
            private set;
        }

        public IReadOnlyCollection<PropertyAccessor> ReadableWritableInstanceProperties
        {
            get; private set;
        }

        public IEnumerable<Tuple<PropertyAccessor, TAttribute>> ReadableWritableInstancePropertiesWith<TAttribute>(Boolean inherit)
            where TAttribute : Attribute
        {
            return ReadableWritableInstanceProperties
                .Select(p => Tuple.Create(p, p.PropertyInfo.GetCustomAttribute<TAttribute>(inherit)))
                .Where(t => t.Item2 != null);
        }

        private void InitReadableWritableInstanceProperties()
        {
            ReadableWritableInstanceProperties = ObjectType
                .GetRuntimeProperties()
                .Where(p => p.CanRead && p.CanWrite && !p.GetMethod.IsStatic && !p.SetMethod.IsStatic)
                .Select(p => new PropertyAccessor(p))
                .ToImmutableArray();
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<HarshObjectMetadata>();
    }
}
