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
                BaseTypes =
                {
                    new CodeTypeReference(
                        "HarshProvisionerCmdlet",
                        new CodeTypeReference(command.ProvisionerType),
                        new CodeTypeReference(command.ContextType)
                    ),
                },
            };

            commandClass.Members.AddRange(
                command.Properties
                .Select(p => CreateProperty(commandClass, p))
                .ToArray()
            );

            commandClass.Members.Add(CreateProcessRecordMethod(command));

            return commandClass;
        }

        private static CodeMemberMethod CreateProcessRecordMethod(ShellployCommand command)
        {
            var varName = "result";

            var method = new CodeMemberMethod()
            {
                Name = "ProcessRecord",
                ReturnType = null,
                Attributes = MemberAttributes.Family | MemberAttributes.Override,
            };

            method.Statements.Add(
                new CodeVariableDeclarationStatement(
                    command.ProvisionerType,
                    varName,
                    new CodeObjectCreateExpression(command.ProvisionerType)
                )
            );
            method.Statements.AddRange(
                command.Properties
                .Where(prop => !prop.SkipAssignment)
                .Select(prop => CreatePropertyAssignment(prop, varName))
                .ToArray()
            );

            if (command.HasChildren)
            {
                method.Statements.Add(
                    new CodeMethodInvokeExpression(
                        new CodeTypeReferenceExpression(command.ClassName),
                        "ProcessChildren",
                        new CodeVariableReferenceExpression(varName),
                        new CodePropertyReferenceExpression(
                            new CodeThisReferenceExpression(),
                            ShellployCommand.ChildrenPropertyName
                        )
                    )
                );
            }

            method.Statements.Add(
                new CodeMethodInvokeExpression(
                    new CodeMethodReferenceExpression(
                        new CodeThisReferenceExpression(),
                        "WriteObject"
                    ),
                    new CodeVariableReferenceExpression(varName)
                )
            );

            return method;
        }

        private static CodeAssignStatement CreatePropertyAssignment(
            ShellployCommandProperty property, 
            String varName
        )
        {
            CodeExpression valueExpression = new CodePropertyReferenceExpression(
                new CodeThisReferenceExpression(),
                property.Name
            );

            if (property.Type == typeof(Boolean))
            {
                valueExpression = new CodeMethodInvokeExpression(valueExpression, "ToBool");
            }

            return new CodeAssignStatement(
                new CodePropertyReferenceExpression(
                    new CodeVariableReferenceExpression(varName),
                    property.Name
                ),
                valueExpression
            );
        }

        private static CodeMemberProperty CreateProperty(CodeTypeDeclaration targetClass, ShellployCommandProperty property)
        {
            var type = property.Type;

            if (type == typeof(Boolean))
            {
                type = typeof(SwitchParameter);
            }

            var codeProperty = new CodeMemberProperty()
            {
                Name = property.Name,
                Type = new CodeTypeReference(type),
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
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