using Destructurama.Attributed;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
            => PropertyAccessor.GetValue(target);

        public PropertyValueSource GetValueSource(ITrackValueSource target)
            => PropertyAccessor.GetValueSource(target);

        public Boolean IsDefined(Type attributeType, Boolean inherit)
            => PropertyInfo.IsDefined(attributeType, inherit);

        public void SetValue(Object target, Object value)
            => PropertyAccessor.SetValue(target, value);

        public void SetValue(ITrackValueSource target, Object value, PropertyValueSource source)
            => PropertyAccessor.SetValue(target, value, source);

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
