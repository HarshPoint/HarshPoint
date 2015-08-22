using System;

namespace HarshPoint.ShellployGenerator.Builders
{
    public sealed class PropertyModelInputObject : PropertyModel
    {
        public PropertyModelInputObject(PropertyModel next)
            : base(next)
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

            return visitor.VisitInputObject(this);
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(PropertyModelInputObject));
    }
}