using System;
using System.CodeDom;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using SMA = System.Management.Automation;

namespace HarshPoint.ShellployGenerator
{
    internal class CommandCodeGenerator
    {
        public CodeCompileUnit GenerateCompileUnit(ShellployCommand command)
        {
            return new CodeCompileUnit()
            {
                Namespaces =
                {
                    new CodeNamespace(command.Namespace)
                    {
                        Types = { CreateClass(command) },
                    },
                },
            };
        }

        private static CodeTypeDeclaration CreateClass(ShellployCommand command)
        {
            var commandClass = new CodeTypeDeclaration(command.ClassName)
            {
                CustomAttributes =
                {
                    { typeof(CmdletAttribute), command.Verb, command.Noun },
                    { typeof(OutputTypeAttribute), command.ProvisionerType },
                },
                IsClass = true,
                TypeAttributes = TypeAttributes.Public | TypeAttributes.Sealed,
            };

            commandClass.Members.AddRange(
                command.Properties
                .Select(p => CreateProperty(commandClass, p))
                .ToArray()
            );

            return commandClass;
        }

        private static CodeMemberProperty CreateProperty(CodeTypeDeclaration targetClass, ShellployCommandProperty property)
        {
            var codeProperty = new CodeMemberProperty()
            {
                Name = property.Name,
                Type = new CodeTypeReference(property.Type),
                Attributes = MemberAttributes.Public | MemberAttributes.Static,
            };

            codeProperty.GenerateBackingField(targetClass);

            foreach (var parameterAttribute in property.ParameterAttributes)
            {
                codeProperty.CustomAttributes.Add(
                    typeof(SMA.ParameterAttribute),
                    parameterAttribute.GetAttributeArguments()
                );
            }

            return codeProperty;
        }

        public static CodeExpression CreateLiteralExpression(Object value)
        {
            var typeValue = value as Type;

            if (typeValue != null)
            {
                return new CodeTypeOfExpression(typeValue);
            }

            return new CodePrimitiveExpression(value);
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<CommandCodeGenerator>();
    }
}