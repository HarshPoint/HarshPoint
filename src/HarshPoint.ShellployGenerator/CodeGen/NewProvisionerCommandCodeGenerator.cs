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

            var seed = Tuple.Create(
                0, 
                (CodeExpression)null,
                ImmutableList<CodeStatement>.Empty
            );

            var result = objects.Aggregate(seed, (acc, obj) =>
            {
                var index = acc.Item1;
                var parent = acc.Item2;
                var statements = acc.Item3;

                var variable = DeclareParentVariable(acc.Item1, obj);
                var variableRef = variable.ToReference();

                statements = statements
                    .Add(variable)
                    .AddRange(
                        CreateAssignments(obj, variableRef)
                    );
                
                if (parent != null)
                {
                    statements = statements.Add(
                        CreateAddChildCall(parent, variableRef)
                    );
                }

                if (obj == objects.Last())
                {
                    statements = statements.Add(
                        CommandCodeGenerator.CreateWriteObjectCall(variableRef)
                    );
                }

                return Tuple.Create(
                    index + 1, 
                    variableRef,
                    statements
                );
            });

            return CommandCodeGenerator.CreateProcessRecord(result.Item3);
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
