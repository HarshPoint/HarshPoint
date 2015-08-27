using HarshPoint.ShellployGenerator.Builders;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using static System.FormattableString;

namespace HarshPoint.ShellployGenerator.CodeGen
{
    public sealed class NewProvisionerCommandCodeGenerator
    {
        public NewProvisionerCommandCodeGenerator(
            NewProvisionerCommandModel newProvisioner
        )
        {
            if (newProvisioner == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(newProvisioner));
            }

            NewProvisionerCommand = newProvisioner;
            Command = newProvisioner.Command;
        }

        public CommandModel Command { get; }

        public NewProvisionerCommandModel NewProvisionerCommand { get; }

        public IEnumerable<CodeMemberMethod> Methods
        {
            get
            {
                yield return CreateProcessRecord();
            }
        }

        public CommandCodeGenerator ToCodeGenerator()
            => new CommandCodeGenerator(Command, Methods);

        private CodeMemberMethod CreateProcessRecord()
        {
            var objects = NewProvisionerCommand.NewObjects.ToImmutableArray();

            var seed = new
            {
                Index = 0,
                Parent = (CodeExpression)null,
                Statements = ImmutableList<CodeStatement>.Empty,
                Root = (CodeExpression)null,
            };

            var result = objects.Aggregate(seed, (acc, obj) =>
            {
                var variable = DeclareParentVariable(acc.Index, obj);
                var variableRef = variable.ToReference();

                var stmts = acc.Statements
                    .Add(variable)
                    .AddRange(
                        CreateAssignments(obj, variableRef)
                    );
                
                if (acc.Parent != null)
                {
                    stmts = stmts.Add(
                        CreateAddChildCall(acc.Parent, variableRef)
                    );
                }

                return new
                {
                    Index = acc.Index + 1,
                    Parent = variableRef,
                    Statements = stmts,
                    Root = acc.Root ?? variableRef
                };
            });

            var statements = result.Statements.Add(
                CommandCodeGenerator.CreateWriteObjectCall(result.Root)
            );

            return CommandCodeGenerator.CreateProcessRecord(statements);
        }

        private static IEnumerable<CodeStatement> CreateAssignments(
            NewObjectCommandModel newObject,
            CodeExpression target
        )
            => new NewObjectCommandCodeGenerator(newObject)
                .CreateNewObjectAssignments(target);

        private static CodeStatement CreateAddChildCall(
            CodeExpression parent,
            CodeExpression child
        ) 
            => new CodeExpressionStatement(
                new CodeMethodInvokeExpression(
                    null,
                    AddChildMethod,
                    parent,
                    child
                )
            );

        private static CodeVariableDeclarationStatement DeclareParentVariable(
            Int32 index,
            NewObjectCommandModel parent
        ) 
            => new CodeVariableDeclarationStatement(
                parent.TargetType,
                Invariant($"obj{index}"),
                new CodeObjectCreateExpression(parent.TargetType)
            );

        private const String AddChildMethod = "AddChild";

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(NewProvisionerCommandCodeGenerator));

    }
}
