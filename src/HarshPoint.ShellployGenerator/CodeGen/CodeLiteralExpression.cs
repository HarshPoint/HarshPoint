using System;
using System.CodeDom;

namespace HarshPoint.ShellployGenerator
{
    internal static class CodeLiteralExpression
    {
        public static CodeExpression Create(Object value)
        {
            if (value == null)
            {
                return Null;
            }

            var expressionValue = value as CodeExpression;
            if (expressionValue != null)
            {
                return expressionValue;
            }

            var typeValue = value as Type;
            if (typeValue != null)
            {
                return new CodeTypeOfExpression(typeValue);
            }

            var type = value.GetType();
            if (type.IsEnum)
            {
                return new CodeFieldReferenceExpression(
                    new CodeTypeReferenceExpression(type),
                    type.GetEnumName(value)
                );
            }

            return new CodePrimitiveExpression(value);
        }

        private static readonly CodeExpression Null
            = new CodePrimitiveExpression(null);
    }
}
