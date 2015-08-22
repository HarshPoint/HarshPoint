using System.CodeDom;
using System.Collections.Generic;
using System.Linq;

namespace HarshPoint.ShellployGenerator
{
    internal static class AttributeDataCodeDom
    {
        public static CodeAttributeDeclaration ToCodeAttributeDeclaration(
            this AttributeData attribute
        )
        {
            if (attribute == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(attribute));
            }

            var result = new CodeAttributeDeclaration(
                new CodeTypeReference(attribute.AttributeType)
            );

            foreach (var ctorArg in attribute.ConstructorArguments)
            {
                result.Arguments.Add(
                    new CodeAttributeArgument(
                        CodeLiteralExpression.Create(ctorArg)
                    )
                );
            }

            foreach (var namedArg in attribute.NamedArguments)
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

        public static CodeAttributeDeclarationCollection ToCodeAttributeDeclarations(
            this IEnumerable<AttributeData> attributes
        )
        {
            if (attributes == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(attributes));
            }

            return new CodeAttributeDeclarationCollection(
                attributes.Select(ToCodeAttributeDeclaration).ToArray()
            );
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(AttributeDataCodeDom));
    }
}
