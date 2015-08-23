namespace HarshPoint.ShellployGenerator.Builders
{
    public sealed class RemoveIgnoredOrUnsynthesizedVisitor : 
        PropertyModelVisitor
    {
        public override PropertyModel Visit(PropertyModel property)
        {
            if (property == null)
            {
                return null;
            }

            if (property.HasElementsOfType<PropertyModelIgnored>())
            {
                return null;
            }

            if (property.HasElementsOfType<PropertyModelSynthesized>())
            {
                return base.Visit(property);
            }

            return null;
        }
    }
}
