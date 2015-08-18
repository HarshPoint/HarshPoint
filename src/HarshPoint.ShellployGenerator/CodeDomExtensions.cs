using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CodeDom;

namespace HarshPoint.ShellployGenerator
{
    internal static class CodeDomExtensions
    {
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
                        tuple => new CodeAttributeArgument(tuple.Item1, CommandCodeGenerator.CreateLiteralExpression(tuple.Item2))
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

        public static void GenerateBackingField(
            this CodeMemberProperty property,
            CodeTypeDeclaration containingType,
            Object defaultValue
        )
        {
            if (containingType == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(containingType));
            }

            if (String.IsNullOrEmpty(property.Name))
            {
                throw Logger.Fatal.Argument(
                    nameof(property),
                    SR.CodeDomExtensions_PropertyNameNotSet
                );
            }

            if (property.GetStatements.Any())
            {
                throw Logger.Fatal.Argument(
                    nameof(property),
                    SR.CodeDomExtensions_PropertyGetterNotEmpty
                );
            }

            if (property.SetStatements.Any())
            {
                throw Logger.Fatal.Argument(
                    nameof(property),
                    SR.CodeDomExtensions_PropertySetterNotEmpty
                );
            }

            var fieldName = "_" + ToCamelCase(property.Name);

            if (containingType.Members
                .Cast<CodeTypeMember>()
                .Any(member => member.Name == fieldName)
            )
            {
                throw Logger.Fatal.InvalidOperation(SR.CodeDomExtensions_FieldExists);
            }

            var isStatic = (property.Attributes & MemberAttributes.Static) == MemberAttributes.Static;

            var codeField = new CodeMemberField()
            {
                Name = fieldName,
                Type = property.Type,
                Attributes = MemberAttributes.Private | (isStatic ? MemberAttributes.Static : 0),
            };

            if (defaultValue != null)
            {
                codeField.InitExpression = CommandCodeGenerator.CreateLiteralExpression(defaultValue);
            }

            CreatePropertyGetterAndSetter(property, codeField.Name, CreateThisExpression(containingType, isStatic));

            containingType.Members.Add(codeField);
        }

        private static void CreatePropertyGetterAndSetter(
            CodeMemberProperty property,
            String fieldName,
            CodeExpression thisExpression
        )
        {
            property.GetStatements.Add(
                new CodeMethodReturnStatement(
                    new CodeFieldReferenceExpression(thisExpression, fieldName)
                )
            );
            property.SetStatements.Add(
                new CodeAssignStatement(
                    new CodeFieldReferenceExpression(thisExpression, fieldName),
                    new CodePropertySetValueReferenceExpression()
                )
            );
        }

        private static CodeExpression CreateThisExpression(CodeTypeDeclaration containingType, Boolean isStatic)
        {
            if (isStatic)
            {
                return new CodeTypeReferenceExpression(containingType.Name);
            }
            else
            {
                return new CodeThisReferenceExpression();
            }
        }

        private static String ToCamelCase(String value)
        {
            if (String.IsNullOrEmpty(value))
            {
                throw Logger.Fatal.ArgumentNullOrEmpty(nameof(value));
            }

            return Char.ToLowerInvariant(value[0]) + value.Substring(1);
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(CodeDomExtensions));
    }
}
