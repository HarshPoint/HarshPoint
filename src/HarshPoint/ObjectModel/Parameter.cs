using Destructurama.Attributed;
using HarshPoint.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;

namespace HarshPoint.ObjectModel
{
    internal sealed class Parameter
    {
        private static readonly HarshLogger Logger = HarshLog.ForContext<Parameter>();

        public Parameter(
            PropertyInfo propertyInfo,
            ParameterAttribute parameterAttribute,
            IEnumerable<ParameterValidationAttribute> validationAttributes
        )
        {
            if (propertyInfo == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(propertyInfo));
            }

            if (parameterAttribute == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(parameterAttribute));
            }

            if (validationAttributes == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(validationAttributes));
            }

            PropertyInfo = propertyInfo;
            ParameterAttribute = parameterAttribute;

            ValidationAttributes = validationAttributes.ToImmutableArray();

            ValidateMandatoryIsNullable();

            Getter = propertyInfo.MakeGetter();
            Setter = propertyInfo.MakeSetter();
        }

        [NotLogged]
        public Func<Object, Object> Getter
        {
            get;
            private set;
        }

        [NotLogged]
        public Boolean IsCommonParameter => (ParameterSetName == null);

        public Boolean IsMandatory => ParameterAttribute.Mandatory;

        public String Name => PropertyInfo.Name;

        public String ParameterSetName => ParameterAttribute.ParameterSetName;

        [NotLogged]
        public PropertyInfo PropertyInfo
        {
            get;
            private set;
        }

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

        public Type ResolvedType
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

        private ParameterAttribute ParameterAttribute
        {
            get;
            set;
        }

        private void ValidateMandatoryIsNullable()
        {
            if (IsMandatory && !PropertyTypeInfo.IsNullable())
            {
                throw Logger.Fatal.ObjectMetadata(
                    SR.HarshProvisionerMetadata_NoValueTypeMandatory,
                    PropertyInfo.DeclaringType,
                    PropertyInfo.Name,
                    PropertyInfo.PropertyType
                );
            }
        }
    }
}
