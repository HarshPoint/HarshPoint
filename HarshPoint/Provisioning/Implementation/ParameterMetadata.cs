using HarshPoint.Reflection;
using System;
using System.Reflection;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ParameterMetadata
    {
        public ParameterMetadata(PropertyInfo propertyInfo, ParameterAttribute parameterAttribute)
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

            Getter = propertyInfo.MakeGetter<Object, Object>();
            Setter = propertyInfo.MakeSetter<Object, Object>();
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
    }
}
