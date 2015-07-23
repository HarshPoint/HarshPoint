using HarshPoint.Reflection;
using System;
using System.Reflection;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ResolvedProperty
    {
        public String Name { get; private set; }

        public Func<Object, Object> Getter { get; private set; }

        public Action<Object, Object> Setter { get; private set; }

        public TypeInfo PropertyTypeInfo { get; private set; }

        public Func<IResolveBuilder> ResolveBuilderFactory { get; private set; }

        public ResolvedProperty(
            String name,
            TypeInfo propertyTypeInfo,
            Func<Object, Object> getter,
            Action<Object, Object> setter,
            Func<IResolveBuilder> resolveBuilderFactory = null
        )
        {
            if (name == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(name));
            }

            if (propertyTypeInfo == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(propertyTypeInfo));
            }

            if (getter == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(getter));
            }

            if (setter == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(setter));
            }

            PropertyTypeInfo = propertyTypeInfo;
            Getter = getter;
            Setter = setter;
            ResolveBuilderFactory = resolveBuilderFactory;
        }

        public ResolvedProperty(
            PropertyInfo property, 
            Func<IResolveBuilder> resolveBuilderFactory = null
        )
        {
            if (property == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(property));
            }

            Name = property.Name;
            PropertyTypeInfo = property.PropertyType.GetTypeInfo();
            Getter = property.MakeGetter();
            Setter = property.MakeSetter();
            ResolveBuilderFactory = resolveBuilderFactory;
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<ResolvedProperty>();
    }
}
