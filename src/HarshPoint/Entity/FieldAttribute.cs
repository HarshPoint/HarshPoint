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
                throw Logger.Fatal.ArgumentNull("fieldId");
            }

            var guid = Guid.Empty;

            if (!Guid.TryParse(fieldId, out guid))
            {
                throw Logger.Fatal.Argument("fieldId", SR.FieldAttribute_InvalidGuid);
            }

            if (guid == Guid.Empty)
            {
                throw Logger.Fatal.Argument("fieldId", SR.FieldAttribute_EmptyGuid);
            }

            FieldId = guid;
        }

        public Guid FieldId { get; }

        private static readonly HarshLogger Logger = HarshLog.ForContext<FieldAttribute>();
    }
}
