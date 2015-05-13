using System;
using System.Reflection;

namespace HarshPoint
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
    }
}
