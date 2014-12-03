using System;
using System.Text.RegularExpressions;

namespace HarshPoint.Entity
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ContentTypeAttribute : Attribute
    {
        public ContentTypeAttribute(String contentTypeId)
        {
            ContentTypeId = contentTypeId;
        }

        public String ContentTypeId
        {
            get;
            private set;
        }
    }
}
