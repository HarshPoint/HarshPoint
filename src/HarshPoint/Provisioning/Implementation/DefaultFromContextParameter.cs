using Destructurama.Attributed;
using System;
using System.Reflection;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class DefaultFromContextParameter
    {
        private static readonly TypeInfo IDefaultFromContextTagTypeInfo =
            typeof(IDefaultFromContextTag).GetTypeInfo();

        private static readonly HarshLogger Logger =
            HarshLog.ForContext<DefaultFromContextParameter>();

        private DefaultFromContextParameter(PropertyInfo property, DefaultFromContextAttribute attribute)
        {
            ResolvedType = Resolvable.GetResolvedType(property.PropertyType);
            TagType = attribute.TagType;

            if (TagType != null)
            {
                if (ResolvedType != null)
                {
                    throw Logger.Fatal.ProvisionerMetadata(
                        SR.HarshProvisionerMetadata_NoTagTypesOnResolvers,
                        property.DeclaringType,
                        property.Name
                    );
                }

                if (!IDefaultFromContextTagTypeInfo.IsAssignableFrom(TagType.GetTypeInfo()))
                {
                    throw Logger.Fatal.ProvisionerMetadata(
                        SR.HarshProvisionerMetadata_TagTypeNotAssignableFromIDefaultFromContextTag,
                        TagType,
                        property.DeclaringType,
                        property.Name
                    );
                }
            }
        }

        [LogAsScalar]
        public Type ResolvedType
        {
            get;
            private set;
        }

        [LogAsScalar]
        public Type TagType
        {
            get;
            private set;
        }

        public static DefaultFromContextParameter FromPropertyInfo(PropertyInfo property)
        {
            var attr = property.GetCustomAttribute<DefaultFromContextAttribute>(inherit: true);

            if (attr != null)
            {
                return new DefaultFromContextParameter(property, attr);
            }

            return null;
        }
    }
}
