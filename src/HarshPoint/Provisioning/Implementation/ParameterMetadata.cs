using HarshPoint.Reflection;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ParameterMetadata
    {
        public ParameterMetadata(
            PropertyInfo propertyInfo,
            ParameterAttribute parameterAttribute,
            DefaultFromContextPropertyInfo defaultFromContext,
            IEnumerable<ParameterValidationAttribute> validation = null
        )
        {
            if (propertyInfo == null)
            {
                throw Error.ArgumentNull(nameof(propertyInfo));
            }

            if (parameterAttribute == null)
            {
                throw Error.ArgumentNull(nameof(parameterAttribute));
            }

            PropertyInfo = propertyInfo;
            ParameterAttribute = parameterAttribute;
            DefaultFromContext = defaultFromContext;

            ValidationAttributes =
                validation?.ToImmutableArray() ??
                ImmutableArray<ParameterValidationAttribute>.Empty;

            Getter = propertyInfo.MakeGetter<Object, Object>();
            Setter = propertyInfo.MakeSetter<Object, Object>();
        }

        public DefaultFromContextPropertyInfo DefaultFromContext
        {
            get;
            private set;
        }

        public Func<Object, Object> Getter
        {
            get;
            private set;
        }

        public String Name => PropertyInfo.Name;

        public ParameterAttribute ParameterAttribute
        {
            get;
            private set;
        }

        public String ParameterSetName => ParameterAttribute.ParameterSetName;

        public PropertyInfo PropertyInfo
        {
            get;
            private set;
        }

        public Type PropertyType => PropertyInfo.PropertyType;

        public TypeInfo PropertyTypeInfo => PropertyType.GetTypeInfo();

        public Action<Object, Object> Setter
        {
            get;
            private set;
        }

        public IReadOnlyList<ParameterValidationAttribute> ValidationAttributes
        {
            get;
            private set;
        }

        public Boolean HasDefaultValue(Object provisioner)
        {
            var value = Getter(provisioner);

            if (value == null)
            {
                return true;
            }

            var str = value as String;

            if ((str != null) && (str.Length == 0))
            {
                return true;
            }

            return false;
        }


    }
}
