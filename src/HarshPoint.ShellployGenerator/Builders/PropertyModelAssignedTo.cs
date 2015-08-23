using System;
using System.Reflection;

namespace HarshPoint.ShellployGenerator.Builders
{
    public sealed class PropertyModelAssignedTo : PropertyModel
    {
        public PropertyModelAssignedTo(
            PropertyInfo targetProperty,
            PropertyModel next
        )
            : base(next)
        {
            if (targetProperty == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(targetProperty));
            }

            TargetPropertyName = targetProperty.Name;
        }

        public PropertyModelAssignedTo(
            String targetPropertyName,
            PropertyModel next
        )
            : base(next)
        {
            if (targetPropertyName == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(targetPropertyName));
            }

            TargetPropertyName = targetPropertyName;
        }

        public String TargetPropertyName { get; }

        protected internal override PropertyModel Accept(PropertyModelVisitor visitor)
        {
            if (visitor == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(visitor));
            }

            return visitor.VisitAssignedTo(this);
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(PropertyModelAssignedTo));
    }
}
