using System;
using System.CodeDom;
using System.Collections.Immutable;

namespace HarshPoint.ShellployGenerator.Builders
{
    public sealed class PropertyModelConditionalFixed
        : PropertyModel, IValuePropertyModel
    {
        internal PropertyModelConditionalFixed(
            IImmutableList<Tuple<CodeExpression, Object>> conditionalValues,
            Object elseValue
        )
        {
            ConditionalValues = conditionalValues;
            ElseValue = elseValue;
        }

        public IImmutableList<Tuple<CodeExpression, Object>> ConditionalValues { get; }

        public Object ElseValue { get; }

        public override PropertyModel InsertIntoContainer(
            PropertyModel existing
        )
        {
            PropertyModelValidator.ValidateDoesNotContain<IValuePropertyModel>(existing, this);
            return base.InsertIntoContainer(existing);
        }

        protected internal override PropertyModel Accept(
            PropertyModelVisitor visitor
        )
        {
            if (visitor == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(visitor));
            }

            return visitor.VisitConditionalFixed(this);
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(PropertyModelConditionalFixed));
    }
}
