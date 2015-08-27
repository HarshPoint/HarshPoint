using System;
namespace HarshPoint.ShellployGenerator.Builders
{
    public sealed class PropertyModelNegated : PropertyModel
    {
        public PropertyModelNegated(
            String positivePropertyName, 
            PropertyModel next
        )
            : base(next)
        {
            if (positivePropertyName == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(positivePropertyName));
            }

            PositivePropertyName = positivePropertyName;
        }

        public String PositivePropertyName { get; }

        protected internal override PropertyModel Accept(PropertyModelVisitor visitor)
        {
            if (visitor == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(visitor));
            }

            return visitor.VisitNegated(this);
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(PropertyModelNegated));
    }
}
