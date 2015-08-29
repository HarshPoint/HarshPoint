using HarshPoint.ShellployGenerator.Builders;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using static System.FormattableString;

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
            FileName = Invariant($"{command.ClassName}.cs");
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

        internal static CodeMemberMethod CreateProcessRecord()
            => new CodeMemberMethod()
            {
                Attributes = FamilyOverride,
                Name = "ProcessRecord",
                ReturnType = Void,
            };

        internal static CodeMemberMethod CreateProcessRecord(
            IEnumerable<CodeStatement> statements
        )
        {
            if (statements == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(statements));
            }

            var method = CreateProcessRecord();

            foreach (var stmt in statements)
            {
                method.Statements.Add(stmt);
            }

            return method;
        }

        
        internal static CodeStatement CreateWriteObjectCall(
            CodeExpression objectExpression
        )
            => new CodeExpressionStatement(
                new CodeMethodInvokeExpression(
                    WriteObject,
                    objectExpression
                )
            );

        private const MemberAttributes FamilyOverride
            = MemberAttributes.Family | MemberAttributes.Override;

        private const MemberAttributes PrivateStatic
            = MemberAttributes.Private | MemberAttributes.Static;

        private static readonly CodeThisReferenceExpression This
            = new CodeThisReferenceExpression();

        private static readonly CodeTypeReference Void
            = new CodeTypeReference(typeof(void));

        private static readonly CodeMethodReferenceExpression WriteObject
            = new CodeMethodReferenceExpression(
                This,
                "WriteObject"
            );

        private static readonly HarshLogger Logger
            = HarshLog.ForContext<CommandCodeGenerator>();
    }
}