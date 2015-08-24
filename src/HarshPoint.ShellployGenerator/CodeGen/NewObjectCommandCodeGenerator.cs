using HarshPoint.ShellployGenerator.Builders;
using System.CodeDom;
using System.Collections.Generic;

namespace HarshPoint.ShellployGenerator.CodeGen
{
    public sealed class NewObjectCommandCodeGenerator
    {
        public NewObjectCommandCodeGenerator(NewObjectCommandModel newObject)
        {
            if (newObject == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(newObject));
            }

            NewObjectCommand = newObject;
            Command = newObject.Command;
        }

        public IEnumerable<CodeStatement> CreateNewObjectAssignments(
            CodeExpression targetObject
        )
        {
            var visitor = new NewObjectAssignmentVisitor(targetObject);
            visitor.Visit(Command.Properties);
            return visitor.Statements;
        }

        public IEnumerable<CodeMemberMethod> Methods
        {
            get
            {
                yield return CreateProcessRecord();
            }
        }

        private CodeMemberMethod CreateProcessRecord()
        {
            var resultDeclaration = new CodeVariableDeclarationStatement(
                NewObjectCommand.TargetType,
                "result",
                new CodeObjectCreateExpression(NewObjectCommand.TargetType)
            );

            var resultRef = new CodeVariableReferenceExpression(
                resultDeclaration.Name
            );

            var assignments = CreateNewObjectAssignments(resultRef);

            var method = CommandCodeGenerator.CreateProcessRecord();

            method.Statements.Add(resultDeclaration);

            foreach (var assign in assignments)
            {
                method.Statements.Add(assign);
            }

            var writeObject = CommandCodeGenerator.CreateWriteObjectCall(
                resultRef
            );

            method.Statements.Add(writeObject);
            return method;
        }

        public CommandModel Command { get; }

        public NewObjectCommandModel NewObjectCommand { get; }

        public CommandCodeGenerator ToCommandCodeGenerator()
            => new CommandCodeGenerator(Command, Methods);

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(NewObjectCommandCodeGenerator));

    }
}
