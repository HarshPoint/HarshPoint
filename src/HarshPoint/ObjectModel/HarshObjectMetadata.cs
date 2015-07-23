using System;
using System.Collections.Generic;
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
                throw Error.ArgumentNull(nameof(type));
            }

            ObjectType = type;
            ObjectTypeInfo = type.GetTypeInfo();
        }

        public HarshObjectMetadata(TypeInfo typeInfo)
        {
            if (typeInfo == null)
            {
                throw Error.ArgumentNull(nameof(typeInfo));
            }

            ObjectType = typeInfo.AsType();
            ObjectTypeInfo = typeInfo;
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

        protected IEnumerable<PropertyInfo> GetPropertiesWith(Type attributeType, Boolean inherit)
        {
            return ObjectType
                .GetRuntimeProperties()
                .Where(p => p.IsDefined(attributeType, inherit));
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        protected IEnumerable<Tuple<PropertyInfo, TAttribute>> GetPropertiesWith<TAttribute>(Boolean inherit)
            where TAttribute : Attribute
        {
            return ObjectType
                .GetRuntimeProperties()
                .Select(p => Tuple.Create(p, p.GetCustomAttribute<TAttribute>(inherit)))
                .Where(t => t.Item2 != null);
        }
    }
}
