namespace HarshPoint.ShellployGenerator.Builders
{
    public sealed class PropertyModelNegated : PropertyModel
    {
        public PropertyModelNegated(PropertyModel next)
            : base(next)
        {
        }

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
