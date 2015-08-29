using System;

namespace HarshPoint.ShellployGenerator.Builders
{
    public sealed class PropertyModelPositional : PropertyModel
    {
        public PropertyModelPositional(Int32 sortOrder)
            : base(sortOrder, null)
        {
        }

        public PropertyModelPositional(Int32 sortOrder, PropertyModel next)
            : base(sortOrder, next)
        {
        }

        protected internal override PropertyModel Accept(
            PropertyModelVisitor visitor
        )
        {
            if (visitor == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(visitor));
            }

            return visitor.VisitPositional(this);
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(PropertyModelPositional));
    }
}
