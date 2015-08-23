namespace HarshPoint.ShellployGenerator.Builders
{
    public sealed class PropertyModelIgnored : PropertyModel
    {
        public PropertyModelIgnored() { }

        public PropertyModelIgnored(PropertyModel next)
            :base(next)
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

            return visitor.VisitIgnored(this);
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(PropertyModelIgnored));
    }
}
