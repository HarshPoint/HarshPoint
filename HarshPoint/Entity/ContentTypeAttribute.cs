using System;
using System.Text.RegularExpressions;

namespace HarshPoint.Entity
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class ContentTypeAttribute : Attribute
    {
        private static readonly Regex RelativeCTIdRegex = new Regex("^([A-Fa-f0-9]{2}|[A-Fa-f0-9]{32})$");

        public ContentTypeAttribute(String relativeContentTypeId)
        {
            if (relativeContentTypeId == null)
            {
                throw Error.ArgumentNull("relativeContentTypeId");
            }

            if (!RelativeCTIdRegex.IsMatch(relativeContentTypeId))
            {
                throw Error.ArgumentOutOfRange("relativeContentTypeId", SR.ContentTypeAttribute_RelCTId_OutOfRange);
            }

            if (StringComparer.Ordinal.Equals(relativeContentTypeId, "00"))
            {
                throw Error.ArgumentOutOfRange("relativeContentTypeId", SR.ContentTypeAttribute_RelCTId_00_OutOfRange);
            }

            RelativeContentTypeId = relativeContentTypeId;
        }

        public String RelativeContentTypeId
        {
            get;
            private set;
        }
    }
}
