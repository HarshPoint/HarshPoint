using HarshPoint.ShellployGenerator.Builders;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Management.Automation;
using System.Reflection;

namespace HarshPoint.ShellployGenerator.CodeGen
{
    public sealed class CommandCodeGenerator : CodeDomGenerator
    {
        public CommandCodeGenerator(CommandModel command)
            : this(command, null)
        {
        }

        public CommandCodeGenerator(
            CommandModel command,
            IEnumerable<CodeMemberMethod> methods
        )
        {
            if (command == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(command));
            }

            if (methods == null)
            {
                methods = Enumerable.Empty<CodeMemberMethod>();
            }

            Command = command;
            Methods = methods.ToImmutableArray();
            FileName = $"{command.ClassName}.cs";
        }

        public CommandModel Command { get; }

        public ImmutableArray<CodeMemberMethod> Methods { get; }

        protected override CodeCompileUnit ToCodeCompileUnit()
            => new CodeCompileUnit()
            {
                Namespaces = { CreateNamespace() },
            };

        private CodeNamespace CreateNamespace()
        {
            var ns = new CodeNamespace(Command.Namespace)
            {
                Types = { ToCodeTypeDeclaration() },
            };

            foreach (var imported in Command.ImportedNamespaces.OrderBy(s => s))
            {
                ns.Imports.Add(new CodeNamespaceImport(imported));
            }

            return ns;
        }

        public CodeTypeDeclaration ToCodeTypeDeclaration()
        {
            var result = new CodeTypeDeclaration(Command.ClassName)
            {
                CustomAttributes =
                    Command.Attributes.ToCodeAttributeDeclarations(),
                IsClass = true,
                TypeAttributes = TypeAttributes.Public | TypeAttributes.Sealed,
            };

            foreach (var baseType in Command.BaseTypes)
            {
                result.BaseTypes.Add(new CodeTypeReference(baseType));
            }

            new DeclarePropertiesVisitor(result).Visit(Command.Properties);

            foreach (var method in Methods)
            {
                result.Members.Add(method);
            }

            return result;
        }

#if false
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
                        $"CreateProvisioner{Command.ProvisionerType.Name}"
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

            var types = Command.ParentProvisionerTypes
                .Concat(new Type[] { Command.ProvisionerType })
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
                    Command.Properties
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
                else if (Command.HasInputObject)
                {
                    method.Statements.Add(
                        new CodeMethodInvokeExpression(
                            null,
                            "AddChildren",
                            resultVar,
                            new CodePropertyReferenceExpression(
                                This,
                                Builders.CommandBuilder.InputObjectPropertyName
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
                valueExpression = CodeLiteralExpression.Create(
                    property.FixedValue
                );
            }
            else if (property.Type == typeof(Boolean))
            {
                valueExpression = new CodeMethodInvokeExpression(
                    valueExpression, "ToBool"
                );
            }


            return new CodeAssignStatement(
                new CodePropertyReferenceExpression(
                    resultVar,
                    property.Identifier
                ),
                valueExpression
            );
        }

        private static CodeMemberProperty CreateProperty(
            CodeTypeDeclaration targetClass,
            ShellployCommandProperty property
        )
        {
            var type = property.Type;

            if (type == typeof(Boolean))
            {
                type = typeof(SwitchParameter); 
            }

            var codeProperty = new CodeMemberProperty()
            {
                Name = property.PropertyName,
                Type = new CodeTypeReference(type),
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
            };

            codeProperty.GenerateBackingField(
                targetClass,
                property.DefaultValue
            );

            codeProperty.CustomAttributes.AddRange(
                property.Attributes
                .Select(a => a.ToCodeAttributeDeclaration())
                .ToArray()
            );

            return codeProperty;
        }
#endif

        private static readonly CodeThisReferenceExpression This
            = new CodeThisReferenceExpression();

        private static readonly HarshLogger Logger
            = HarshLog.ForContext<CommandCodeGenerator>();

    }
}