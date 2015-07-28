using System;
using System.Reflection;

namespace HarshPoint.Entity.Metadata
{
    internal sealed class ContentTypeIdBuilder
    {
        private HarshContentTypeId _result;
        private TypeInfo _entityTypeInfo;

        public ContentTypeIdBuilder(TypeInfo entityTypeInfo)
        {
            if (entityTypeInfo == null)
            {
                throw Logger.Fatal.ArgumentNull("entityTypeInfo");
            }

            _entityTypeInfo = entityTypeInfo;
        }

        public override String ToString()
        {
            if (_result == null)
            {
                _result = Build(_entityTypeInfo);
            }

            return _result.ToString();
        }

        private HarshContentTypeId Build(TypeInfo t)
        {
            if (t.AsType() == typeof(HarshEntity))
            {
                // don't recurse up to Object. we should never get 
                // here anyway, since entities are supposed to inherit from
                // something with an absolute ID specified,
                // not directly from the HarshEntity class. 
                return null;
            }

            var cta = t.GetCustomAttribute<ContentTypeAttribute>(inherit: false);

            if (cta == null)
            {
                if (t == _entityTypeInfo)
                {
                    throw Logger.Fatal.InvalidOperationFormat(
                        SR.ContentTypeIdBuilder_NoContentTypeAttribute,
                        t.FullName
                    );
                }
                else
                {
                    throw Logger.Fatal.InvalidOperationFormat(
                        SR.ContentTypeIdBuilder_NoContentTypeAttributeBaseClass,
                        t.FullName,
                        _entityTypeInfo.FullName
                    );
                }
            }

            var ctid = HarshContentTypeId.Parse(cta.ContentTypeId);

            if (ctid.IsAbsolute)
            {
                // an absolute ID. do not recurse further up the
                // class hierarchy, take it as it is
                return ctid;
            }
            else
            {
                // not an absolute ID, append the parent type ID first
                var result = Build(t.BaseType.GetTypeInfo());

                if (result == null)
                {
                    throw Logger.Fatal.InvalidOperationFormat(
                        SR.ContentTypeIdBuilder_NoAbsoluteIDInHierarchy,
                        _entityTypeInfo.FullName
                    );
                }

                return result.Append(ctid);
            }
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<ContentTypeIdBuilder>();
    }
}
