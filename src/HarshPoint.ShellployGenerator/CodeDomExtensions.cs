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
        {
            return attributeCollection.Add(
                attributeType,
                parameters.Select(
                    param => Tuple.Create<String, Object>(null, param)
                ).ToArray()
            );
        }

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

        public static void GenerateBackingField(this CodeMemberProperty property, CodeTypeDeclaration containingType)
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

        private static CodeExpression CreateThisExpression(CodeTypeDeclaration containingType, bool isStatic)
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
