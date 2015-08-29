namespace HarshPoint.ShellployGenerator.Builders
{
    public sealed class RemoveIgnoredOrUnsynthesizedVisitor : 
        PropertyModelVisitor
    {
        public override PropertyModel Visit(PropertyModel propertyModel)
        {
            if (propertyModel == null)
            {
                return null;
            }

            if (propertyModel.HasElementsOfType<PropertyModelIgnored>())
            {
                return null;
            }

            if (propertyModel.HasElementsOfType<PropertyModelSynthesized>())
            {
                return base.Visit(propertyModel);
            }

            return null;
        }
    }
}
