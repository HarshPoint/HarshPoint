using System;
using System.CodeDom;
using System.Linq;

namespace HarshPoint.ShellployGenerator
{
    internal static class CodeDomExtensions
    {
        public static CodeStatement ToStatement(
            this CodeExpression expression
        )
        {
            if (expression == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(expression));
            }

            return new CodeExpressionStatement(expression);
        }

        public static CodeExpression ToReference(
            this CodeVariableDeclarationStatement declaration
        )
        {
            if (declaration == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(declaration));
            }

            return new CodeVariableReferenceExpression(
                declaration.Name
            );
        }

        public static Int32 Add(
            this CodeAttributeDeclarationCollection attributeCollection,
            Type attributeType,
            params Object[] parameters
        )
            => attributeCollection.Add(
                attributeType,
                parameters.Select(
                    param => Tuple.Create<String, Object>(null, param)
                ).ToArray()
            );

        public static Int32 Add(
            this CodeAttributeDeclarationCollection attributeCollection,
            Type attributeType,
            params Tuple<String, Object>[] parameters
        )
        {
            if (attributeType == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(attributeType));
            }

            return attributeCollection.Add(
                new CodeAttributeDeclaration(
                    new CodeTypeReference(attributeType),
                    parameters.Select(
                        tuple => new CodeAttributeArgument(
                            tuple.Item1,
                            CodeLiteralExpression.Create(tuple.Item2)
                        )
                    ).ToArray()
                )
            );
        }

        public static CodeExpression Call(
            this CodeExpression targetObject,
            String methodName
        )
            => targetObject.Call(methodName, new Type[0], new CodeExpression[0]);

        public static CodeExpression Call(
            this CodeExpression targetObject,
            String methodName,
            params CodeExpression[] parameters
        )
            => targetObject.Call(methodName, new Type[0], parameters);

        public static CodeExpression Call(
            this CodeExpression targetObject,
            String methodName,
            params Type[] typeParameters
        )
            => targetObject.Call(methodName, typeParameters, new CodeExpression[0]);

        public static CodeExpression Call(
            this CodeExpression targetObject,
            String methodName,
            Type[] typeParameters,
            params CodeExpression[] parameters
        )
        {
            if (targetObject == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(targetObject));
            }

            if (methodName == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(methodName));
            }

            if (typeParameters == null)
            {
                typeParameters = new Type[0];
            }

            if (parameters == null)
            {
                parameters = new CodeExpression[0];
            }

            var method = new CodeMethodReferenceExpression(
                targetObject,
                methodName,
                typeParameters.Select(
                    t => new CodeTypeReference(t)
                ).ToArray()
            );

            return new CodeMethodInvokeExpression(method, parameters);
        }

        public static CodeExpression IsNotNull(
            this CodeExpression targetObject
        )
        {
            if (targetObject == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(targetObject));
            }

            return new CodeBinaryOperatorExpression(
                targetObject,
                CodeBinaryOperatorType.IdentityInequality,
                new CodePrimitiveExpression(null)
            );
        }

        public static CodeExpression IsNotNullOrEmpty(
            this CodeExpression targetObject
        )
        {
            if (targetObject == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(targetObject));
            }

            return new CodeTypeReferenceExpression(typeof(String))
                .Call(
                    nameof(String.IsNullOrEmpty), targetObject
                );
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(CodeDomExtensions));
    }
}
