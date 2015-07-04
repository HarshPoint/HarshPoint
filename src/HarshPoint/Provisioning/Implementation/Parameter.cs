using Destructurama.Attributed;
using HarshPoint.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class Parameter
    {
        public Parameter(
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

        [NotLogged]
        public Func<Object, Object> Getter
        {
            get;
            private set;
        }

        [NotLogged]
        public Boolean IsCommonParameter => (ParameterSetName == null);
        
        public String Name => PropertyInfo.Name;

        [NotLogged]
        public ParameterAttribute ParameterAttribute
        {
            get;
            private set;
        }

        public String ParameterSetName => ParameterAttribute.ParameterSetName;

        [NotLogged]
        public PropertyInfo PropertyInfo
        {
            get;
            private set;
        }

        [LogAsScalar]
        public Type PropertyType => PropertyInfo.PropertyType;

        [NotLogged]
        public TypeInfo PropertyTypeInfo => PropertyType.GetTypeInfo();

        [NotLogged]
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

            var enumerable = value as IEnumerable;
            if (enumerable != null)
            {
                return !enumerable.Any();
            }

            return false;
        }

        public override String ToString()
            => ParameterSetName == null ?
                PropertyInfo.ToString() :
                PropertyInfo.ToString() + " (" + ParameterSetName + ')';
    }
}
