using System.CodeDom;

namespace HarshPoint.ShellployGenerator
{
    internal static class AttributeDataCodeDom
    {
        public static CodeAttributeDeclaration ToCodeAttributeDeclaration(
            this AttributeData data
        )
        {
            if (data == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(data));
            }

            var result = new CodeAttributeDeclaration(
                new CodeTypeReference(data.AttributeType)
            );

            foreach (var ctorArg in data.ConstructorArguments)
            {
                result.Arguments.Add(
                    new CodeAttributeArgument(
                        CodeLiteralExpression.Create(ctorArg)
                    )
                );
            }

            foreach (var namedArg in data.NamedArguments)
            {
                result.Arguments.Add(
                    new CodeAttributeArgument(
                        namedArg.Key,
                        CodeLiteralExpression.Create(namedArg.Value)
                    )
                );
            }

            return result;
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(AttributeDataCodeDom));
    }
}
