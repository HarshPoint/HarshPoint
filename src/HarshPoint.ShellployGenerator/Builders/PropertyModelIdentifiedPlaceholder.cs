using System;

namespace HarshPoint.ShellployGenerator.Builders
{
    internal sealed class PropertyModelIdentifiedPlaceholder : PropertyModel
    {
        public PropertyModelIdentifiedPlaceholder(String identifier)
            : base(identifier: identifier)
        {
        }

        protected internal override PropertyModel Accept(PropertyModelVisitor visitor)
        {
            if (visitor == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(visitor));
            }

            return visitor.VisitIdentifiedPlaceholder(this);
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(PropertyModelIdentifiedPlaceholder));
    }
}
