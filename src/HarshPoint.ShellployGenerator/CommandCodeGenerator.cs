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
        private static readonly String BaseTypeName = "HarshProvisionerCmdlet";

        private ShellployCommand _command;

        public CommandCodeGenerator(ShellployCommand command)
        {
            _command = command;
        }

        public CodeCompileUnit GenerateCompileUnit()
        {
            return new CodeCompileUnit()
            {
                Namespaces =
                {
                    CreateNamespace()
                },
            };
        }

        private CodeNamespace CreateNamespace()
        {
            var ns = new CodeNamespace(_command.Namespace)
            {
                Types = { CreateClass() },
            };

            ns.Imports.AddRange(
                _command.Usings
                    .Select(n => new CodeNamespaceImport(n))
                    .ToArray()
            );

            return ns;
        }

        private CodeTypeDeclaration CreateClass()
        {
            var commandClass = new CodeTypeDeclaration(_command.ClassName)
            {
                CustomAttributes =
                {
                    {
                        typeof(CmdletAttribute),
                        new CodeFieldReferenceExpression(
                            new CodeTypeReferenceExpression( _command.Verb.Item1),
                            _command.Verb.Item2
                        ),
                        _command.Noun
                    },
                    {
                        typeof(OutputTypeAttribute),
                        _command.ProvisionerType
                    },
                },
                IsClass = true,
                TypeAttributes = TypeAttributes.Public | TypeAttributes.Sealed,
                BaseTypes =
                {
                    new CodeTypeReference(BaseTypeName),
                },
            };

            commandClass.Members.AddRange(
                _command.Properties
                .Where(p => !p.HasFixedValue)
                .Select(p => CreateProperty(commandClass, p))
                .ToArray()
            );

            commandClass.Members.AddRange(CreateProvisionerMethods().ToArray());

            return commandClass;
        }

        private CodeMemberMethod CreateProcessRecordMethod()
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
                        $"CreateProvisioner{_command.ProvisionerType.Name}"
                    )
                )
            );

            return method;
        }

        private IEnumerable<CodeMemberMethod> CreateProvisionerMethods()
        {
            var methods = new List<CodeMemberMethod>();
            methods.Add(CreateProcessRecordMethod());

            var resultVar = new CodeVariableReferenceExpression("result");
            var innerVar = new CodeVariableReferenceExpression("inner");

            var types = _command.ParentProvisionerTypes
                .Concat(new Type[] { _command.ProvisionerType })
                .Reverse()
                .ToArray();

            for (var i = 0; i < types.Length; i++)
            {
                var type = types[i];
                var previousType = 0 < i ? types[i - 1] : null;
                var nextType = i < (types.Length - 1) ? types[i + 1] : null;

                var method = new CodeMemberMethod()
                {
                    Name = $"CreateProvisioner{type.Name}",
                    ReturnType = new CodeTypeReference(typeof(Object)),
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
                    _command.Properties
                    .Where(prop => prop.ProvisionerType == type)
                    .Select(prop => CreatePropertyAssignment(prop, resultVar))
                    .ToArray()
                );

                if (previousType != null)
                {
                    method.Statements.Add(
                        new CodeMethodInvokeExpression(
                            null,
                            "AddChild",
                            resultVar, innerVar
                        )
                    );
                }
                else if (_command.HasInputObject)
                {
                    method.Statements.Add(
                        new CodeMethodInvokeExpression(
                            null,
                            "AddChildren",
                            resultVar,
                            new CodePropertyReferenceExpression(
                                This,
                                ShellployCommand.InputObjectPropertyName
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
                property.PropertyName ?? property.Identifier
            );

            if (property.HasFixedValue)
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
                    property.Identifier
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
                Name = property.PropertyName ?? property.Identifier,
                Type = new CodeTypeReference(type),
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
            };

            codeProperty.GenerateBackingField(targetClass, property.DefaultValue);

            codeProperty.CustomAttributes.AddRange(
                property.Attributes
                .Select(a => a.ToCodeAttributeDeclaration())
                .ToArray()
            );

            return codeProperty;
        }

        public static CodeExpression CreateLiteralExpression(Object value)
        {
            return CodeLiteralExpression.Create(value);
        }

        private static readonly CodeThisReferenceExpression This = new CodeThisReferenceExpression();

        private static readonly HarshLogger Logger = HarshLog.ForContext<CommandCodeGenerator>();
    }
}