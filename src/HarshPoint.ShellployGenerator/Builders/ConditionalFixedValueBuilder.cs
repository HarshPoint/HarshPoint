using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace HarshPoint.ShellployGenerator.Builders
{
    public sealed class ConditionalFixedValueBuilder
    {
        private readonly List<Tuple<CodeExpression, Object>> _conditionalValues
             = new List<Tuple<CodeExpression, Object>>();

        private Object _elseValue;

        public void When(
            CodeExpression condition,
            Object value
        )
        {
            if (condition == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(condition));
            }

            _conditionalValues.Add(Tuple.Create(
                condition,
                value
            ));
        }

        public void Else(Object value)
        {
            _elseValue = value;
        }

        public PropertyModelConditionalFixed ToModel()
            => new PropertyModelConditionalFixed(
                _conditionalValues.ToImmutableArray(),
                _elseValue
            );


        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ConditionalFixedValueBuilder));
    }
}