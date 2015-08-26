namespace HarshPoint.ShellployGenerator.Builders
{
    public sealed class PropertyModelNoNegative : PropertyModel
    {
        protected internal override PropertyModel Accept(PropertyModelVisitor visitor)
        {
            if (visitor == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(visitor));
            }

            return visitor.VisitNoNegative(this);
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(PropertyModelNoNegative));
    }
}
