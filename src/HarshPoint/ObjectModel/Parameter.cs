using Destructurama.Attributed;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Reflection;


namespace HarshPoint.ObjectModel
{
    using static HarshFormattable;

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
        public IDefaultValuePolicy DefaultValuePolicy { get; }

        [NotLogged]
        public PropertyAccessor PropertyAccessor { get; }

        public IReadOnlyList<ParameterValidationAttribute> ValidationAttributes
        {
            get;

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

        public Object GetValue(Object target)
        {
            if (target == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(target));
            }

            return PropertyAccessor.Getter(target);
        }

        public Boolean IsDefined(Type attributeType, Boolean inherit)
            => PropertyInfo.IsDefined(attributeType, inherit);

        public void SetValue(Object target, Object value)
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

            var value = GetValue(target);
            return DefaultValuePolicy.IsDefaultValue(value);
        }

        public override String ToString()
            => ParameterSetName == null ?
                PropertyAccessor.ToString() :
                Invariant($"{PropertyAccessor} ParameterSetName={ParameterSetName}");

        private ParameterAttribute ParameterAttribute { get; }
    }
}
