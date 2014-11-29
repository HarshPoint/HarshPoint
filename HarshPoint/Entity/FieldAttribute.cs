using System;
using System.Diagnostics.CodeAnalysis;

namespace HarshPoint.Entity
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    [SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments")]
    public sealed class FieldAttribute : Attribute
    {
        public FieldAttribute(String fieldId)
        {
            if (fieldId == null)
            {
                throw Error.ArgumentNull("fieldId");
            }

            var guid = Guid.Empty;

            if (!Guid.TryParse(fieldId, out guid))
            {
                throw Error.ArgumentOutOfRange("fieldId", fieldId, SR.FieldAttribute_InvalidGuid);
            }

            if (guid == Guid.Empty)
            {
                throw Error.ArgumentOutOfRange("fieldId", fieldId, SR.FieldAttribute_EmptyGuid);
            }

            FieldId = guid;
        }

        public Guid FieldId
        {
            get;
            private set;
        }
    }
}
