using System;

namespace HarshPoint.ShellployGenerator.Builders
{
    public sealed class PropertyModelRenamed : PropertyModel
    {
        internal PropertyModelRenamed(String propertyName)
        {
            PropertyName = propertyName;
        }

        public String PropertyName { get; }
        
        protected internal override PropertyModel Accept(
            PropertyModelVisitor visitor
        )
        {
            if (visitor == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(visitor));
            }

            return visitor.VisitRenamed(this);
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(PropertyModelRenamed));
    }
}
