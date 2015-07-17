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
        private static readonly HarshLogger Logger = HarshLog.ForContext<Parameter>();

        private static readonly TypeInfo IDefaultFromContextTagTypeInfo =
            typeof(IDefaultFromContextTag).GetTypeInfo();

        public Parameter(
            PropertyInfo propertyInfo,
            ParameterAttribute parameterAttribute,
            DefaultFromContextAttribute defaultFromContextAttribute,
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
            DefaultFromContextAttribute = defaultFromContextAttribute;

            ResolvedType = Resolvable.GetResolvedType(PropertyTypeInfo);
            ValidationAttributes = validationAttributes.ToImmutableArray();

            ValidateDefaultFromContextIsNullable();
            ValidateDefaultFromContextTagTypeIsIDefaultFromContextTag();
            ValidateDefaultFromContextWithTagNotResolvable();
            ValidateMandatoryIsNullable();

            Getter = propertyInfo.MakeGetter();
            Setter = propertyInfo.MakeSetter();

            if (ResolvedType != null)
            {
                ResolveRunnerDefinition = new ResolveRunnerDefinition(
                    Name,
                    PropertyTypeInfo,
                    Getter,
                    Setter
                );
            }
        }

        public Type DefaultFromContextTagType => DefaultFromContextAttribute?.TagType;

        [NotLogged]
        public Func<Object, Object> Getter
        {
            get;
            private set;
        }

        [NotLogged]
        public Boolean IsCommonParameter => (ParameterSetName == null);

        public Boolean IsDefaultFromContext => (DefaultFromContextAttribute != null);

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

        public ResolveRunnerDefinition ResolveRunnerDefinition
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

        private DefaultFromContextAttribute DefaultFromContextAttribute
        {
            get;
            set;
        }

        private ParameterAttribute ParameterAttribute
        {
            get;
            set;
        }

        private void ValidateMandatoryIsNullable()
        {
            if (IsMandatory && !IsNullable(PropertyTypeInfo))
            {
                throw Logger.Fatal.ProvisionerMetadata(
                    SR.HarshProvisionerMetadata_NoValueTypeMandatory,
                    PropertyInfo.DeclaringType,
                    PropertyInfo.Name,
                    PropertyInfo.PropertyType
                );
            }
        }

        private void ValidateDefaultFromContextIsNullable()
        {
            if (IsDefaultFromContext && !IsNullable(PropertyTypeInfo))
            {
                throw Logger.Fatal.ProvisionerMetadata(
                    SR.HarshProvisionerMetadata_NoValueTypeDefaultFromContext,
                    PropertyInfo.DeclaringType,
                    PropertyInfo.Name,
                    PropertyInfo.PropertyType
                );
            }
        }

        private void ValidateDefaultFromContextWithTagNotResolvable()
        {
            if ((DefaultFromContextTagType != null) && (ResolvedType != null))
            {
                throw Logger.Fatal.ProvisionerMetadata(
                    SR.HarshProvisionerMetadata_NoTagTypesOnResolvers,
                    PropertyInfo.DeclaringType,
                    PropertyInfo.Name
                );
            }
        }

        private void ValidateDefaultFromContextTagTypeIsIDefaultFromContextTag()
        {
            if (DefaultFromContextTagType == null)
            {
                return;
            }

            if (!IDefaultFromContextTagTypeInfo.IsAssignableFrom(DefaultFromContextTagType.GetTypeInfo()))
            {
                throw Logger.Fatal.ProvisionerMetadata(
                    SR.HarshProvisionerMetadata_TagTypeNotAssignableFromIDefaultFromContextTag,
                    DefaultFromContextTagType,
                    PropertyInfo.DeclaringType,
                    PropertyInfo.Name
                );
            }
        }

        private static Boolean IsNullable(TypeInfo typeInfo)
        {
            if (typeInfo.IsValueType)
            {
                return Nullable.GetUnderlyingType(typeInfo.AsType()) != null;
            }

            return true;
        }
    }
}
