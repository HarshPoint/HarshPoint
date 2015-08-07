using Destructurama.Attributed;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;

namespace HarshPoint.ObjectModel
{
    public sealed class Parameter
    {
        private static readonly HarshLogger Logger = HarshLog.ForContext<Parameter>();

        internal Parameter(
            PropertyAccessor propertyAccessor,
            ParameterAttribute parameterAttribute,
            IDefaultValuePolicy defaultValuePolicy
        )
        {
            if (propertyAccessor == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(propertyAccessor));
            }

            if (parameterAttribute == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(parameterAttribute));
            }

            if (defaultValuePolicy == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(defaultValuePolicy));
            }

            PropertyAccessor = propertyAccessor;
            ParameterAttribute = parameterAttribute;
            DefaultValuePolicy = defaultValuePolicy;

            ValidationAttributes = PropertyAccessor.PropertyInfo
                .GetCustomAttributes<ParameterValidationAttribute>(inherit: true)
                .ToImmutableArray();
        }

        [NotLogged]
        public IDefaultValuePolicy DefaultValuePolicy { get; private set; }

        [NotLogged]
        public PropertyAccessor PropertyAccessor { get; private set; }

        public IReadOnlyList<ParameterValidationAttribute> ValidationAttributes
        {
            get;
            private set;
        }

        [NotLogged]
        public Boolean IsCommonParameter => (ParameterSetName == null);

        public Boolean IsMandatory => ParameterAttribute.Mandatory;

        public String Name => PropertyAccessor.Name;

        public String ParameterSetName => ParameterAttribute.ParameterSetName;

        [NotLogged]
        public PropertyInfo PropertyInfo => PropertyAccessor.PropertyInfo;

        public Type PropertyType => PropertyAccessor.PropertyType;

        [NotLogged]
        public TypeInfo PropertyTypeInfo => PropertyType.GetTypeInfo();

        public Object Getter(Object target)
        {
            if (target == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(target));
            }

            return PropertyAccessor.Getter(target);
        }

        public Boolean HasCustomAttribute<TAttribute>(Boolean inherit)
            where TAttribute : Attribute
            => PropertyInfo.GetCustomAttribute<TAttribute>(inherit) != null;

        public void Setter(Object target, Object value)
        {
            if (target == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(target));
            }

            PropertyAccessor.Setter(target, value);
        }

        public Boolean HasDefaultValue(Object target)
        {
            if (target == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(target));
            }

            var value = Getter(target);
            return DefaultValuePolicy.IsDefaultValue(value);
        }

        public override String ToString()
            => ParameterSetName == null ?
                PropertyAccessor.ToString() :
                $"{PropertyAccessor} ParameterSetName={ParameterSetName})";

        private ParameterAttribute ParameterAttribute { get; set; }
    }
}
