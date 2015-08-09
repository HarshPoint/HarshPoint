using HarshPoint.Provisioning.Implementation;
using System;
using System.CodeDom;
using System.Collections.Generic;
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
                    CreateNamespace(command)
                }
            };
        }

        private static CodeNamespace CreateNamespace(ShellployCommand command)
        {
            var ns = new CodeNamespace(command.Namespace)
            {
                Types = { CreateClass(command) },
            };

            ns.Imports.AddRange(
                command.Usings
                    .Select(n => new CodeNamespaceImport(n))
                    .ToArray()
            );

            return ns;
        }

        private static CodeTypeDeclaration CreateClass(ShellployCommand command)
        {
            var commandClass = new CodeTypeDeclaration(command.ClassName)
            {
                CustomAttributes =
                {
                    { typeof(CmdletAttribute), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression( command.Verb.Item1), command.Verb.Item2), command.Noun },
                    { typeof(OutputTypeAttribute), command.ProvisionerType },
                },
                IsClass = true,
                TypeAttributes = TypeAttributes.Public | TypeAttributes.Sealed,
                BaseTypes =
                {
                    typeof(PSCmdlet),
                },
            };

            commandClass.Members.AddRange(
                command.Properties
                .Where(p => !p.UseFixedValue)
                .Select(p => CreateProperty(commandClass, p))
                .ToArray()
            );

            commandClass.Members.AddRange(CreateProvisionerMethods(command).ToArray());

            return commandClass;
        }

        private static CodeMemberMethod CreateProcessRecordMethod(ShellployCommand command)
        {
            var method = new CodeMemberMethod()
            {
                Name = "ProcessRecord",
                ReturnType = null,
                Attributes = MemberAttributes.Family | MemberAttributes.Override,
            };

            method.Statements.Add(
                new CodeMethodInvokeExpression(
                    new CodeMethodReferenceExpression(
                        This,
                        "WriteObject"
                    ),
                    new CodeMethodInvokeExpression(
                        This,
                        $"CreateProvisioner{command.ProvisionerType.Name}"
                    )
                )
            );

            return method;
        }

        public static IEnumerable<CodeMemberMethod> CreateProvisionerMethods(ShellployCommand command)
        {
            var methods = new List<CodeMemberMethod>();
            methods.Add(CreateProcessRecordMethod(command));

            var resultVar = new CodeVariableReferenceExpression("result");
            var innerVar = new CodeVariableReferenceExpression("inner");

            var types = command.ParentProvisionerTypes
                .Concat(new Type[] { command.ProvisionerType })
                .Reverse().ToArray();

            for (int i = 0; i < types.Length; i++)
            {
                var type = types[i];
                var previousType = 0 < i ? types[i - 1] : null;
                var nextType = i < (types.Length - 1) ? types[i + 1] : null;

                var method = new CodeMemberMethod()
                {
                    Name = $"CreateProvisioner{type.Name}",
                    ReturnType = new CodeTypeReference(typeof(object)),
                    Attributes = MemberAttributes.Private,
                };

                if (previousType != null)
                {
                    method.Parameters.Add(
                        new CodeParameterDeclarationExpression(previousType, innerVar.VariableName)
                    );
                }

                method.Statements.Add(
                    new CodeVariableDeclarationStatement(
                        type,
                        resultVar.VariableName,
                        new CodeObjectCreateExpression(type)
                    )
                );
                method.Statements.AddRange(
                    command.Properties
                    .Where(prop => prop.AssignmentOnType == type)
                    .Where(prop => !prop.Custom)
                    .Select(prop => CreatePropertyAssignment(prop, resultVar))
                    .ToArray()
                );

                if (previousType != null)
                {
                    method.Statements.Add(
                        new CodeMethodInvokeExpression(
                            new CodeTypeReferenceExpression("HarshProvisionerTreeBuilder"),
                            "AddChild",
                            resultVar, innerVar
                        )
                    );
                }
                else if (command.HasChildren)
                {
                    method.Statements.Add(
                        new CodeMethodInvokeExpression(
                            new CodeTypeReferenceExpression("HarshProvisionerTreeBuilder"),
                            "AddChildren",
                            resultVar,
                            new CodePropertyReferenceExpression(
                                This,
                                ShellployCommand.ChildrenPropertyName
                            )
                        )
                    );
                }

                if (nextType != null)
                {
                    method.Statements.Add(
                        new CodeMethodReturnStatement(
                            new CodeMethodInvokeExpression(
                                This,
                                $"CreateProvisioner{nextType.Name}",
                                resultVar
                            )
                        )
                    );
                }
                else
                {
                    method.Statements.Add(
                        new CodeMethodReturnStatement(resultVar)
                    );
                }

                methods.Add(method);
                previousType = type;
            }

            return methods;
        }

        private static CodeAssignStatement CreatePropertyAssignment(
            ShellployCommandProperty property,
            CodeExpression resultVar
        )
        {
            CodeExpression valueExpression = new CodePropertyReferenceExpression(
                This,
                property.Name
            );

            if (property.UseFixedValue)
            {
                valueExpression = CreateLiteralExpression(property.FixedValue);
            }
            else if (property.Type == typeof(Boolean))
            {
                valueExpression = new CodeMethodInvokeExpression(valueExpression, "ToBool");
            }


            return new CodeAssignStatement(
                new CodePropertyReferenceExpression(
                    resultVar,
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

            codeProperty.GenerateBackingField(targetClass, property.DefaultValue);

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

            var type = value?.GetType();
            if (type?.IsEnum == true)
            {
                return new CodeFieldReferenceExpression(
                    new CodeTypeReferenceExpression(type),
                    type.GetEnumName(value)
                );
            }

            return new CodePrimitiveExpression(value);
        }

        private static readonly CodeThisReferenceExpression This = new CodeThisReferenceExpression();

        private static readonly HarshLogger Logger = HarshLog.ForContext<CommandCodeGenerator>();
    }
}