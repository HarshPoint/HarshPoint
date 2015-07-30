using Serilog.Core;
using Serilog.Events;
using System;
using System.Linq.Expressions;

namespace HarshPoint.Diagnostics
{
    public sealed class ExpressionScalarConversionPolicy : IScalarConversionPolicy, IDestructuringPolicy
    {
        public Boolean TryConvertToScalar(Object value, ILogEventPropertyValueFactory propertyValueFactory, out ScalarValue result)
        {
            var expression = (value as Expression);

            if (expression != null)
            {
                result = new ScalarValue(expression.ToString());
                return true;
            }

            result = null;
            return false;
        }

        public Boolean TryDestructure(Object value, ILogEventPropertyValueFactory propertyValueFactory, out LogEventPropertyValue result)
        {
            ScalarValue scalar;

            if (TryConvertToScalar(value, propertyValueFactory, out scalar))
            {
                result = scalar;
                return true;
            }

            result = null;
            return false;
        }
    }
}
