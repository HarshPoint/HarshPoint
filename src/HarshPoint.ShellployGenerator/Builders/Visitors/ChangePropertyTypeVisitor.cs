using System;

namespace HarshPoint.ShellployGenerator.Builders
{
    public sealed class ChangePropertyTypeVisitor : PropertyModelVisitor
    {
        public ChangePropertyTypeVisitor(Type fromType, Type toType)
        {
            if (fromType == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(fromType));
            }

            if (toType == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(toType));
            }

            FromType = fromType;
            ToType = toType;
        }
        public Type FromType { get; }
        public Type ToType { get; }

        protected internal override PropertyModel VisitSynthesized(
            PropertyModelSynthesized property
        )
        {
            if (property == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(property));
            }

            if (property.PropertyType == FromType)
            {
                return new PropertyModelSynthesized(
                    property.Identifier,
                    ToType,
                    property.Attributes
                );
            }

            return base.VisitSynthesized(property);
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ChangePropertyTypeVisitor));
    }
}
