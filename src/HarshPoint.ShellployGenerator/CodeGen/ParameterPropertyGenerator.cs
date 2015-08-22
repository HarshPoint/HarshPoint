using HarshPoint.ShellployGenerator.Builders;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarshPoint.ShellployGenerator.CodeGen
{
    internal sealed class ParameterPropertyGenerator : PropertyModelVisitor
    {
        private readonly HarshScopedValue<String> _renaming
            = new HarshScopedValue<String>();

        public ParameterPropertyGenerator(CodeTypeDeclaration typeDeclaration)
        {
            if (typeDeclaration == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(typeDeclaration));
            }

            TypeDeclaration = typeDeclaration;
        }

        public CodeTypeDeclaration TypeDeclaration { get; }

        protected internal override PropertyModel VisitIgnored(
            PropertyModelIgnored ignoredBuilder
        )
            => null; // do not generate properties for ignored parameters

        protected internal override PropertyModel VisitFixed(
            PropertyModelFixed fixedBuilder
        )
            => null; // do not generate properties for fixed parameters

        protected internal override PropertyModel VisitRenamed(
            PropertyModelRenamed renamedBuilder
        )
        {
            using (_renaming.EnterIfDefault(renamedBuilder.PropertyName))
            {
                return base.VisitRenamed(renamedBuilder);
            }
        }

        protected internal override PropertyModel VisitSynthesized(
            PropertyModelSynthesized synthesizedBuilder
        )
        {
            var name = _renaming.Value ?? synthesizedBuilder.Identifier;
            var type = new CodeTypeReference(synthesizedBuilder.PropertyType);

            var property = new CodeMemberProperty()
            {
                Name = name,
                Type = type,
            };

            TypeDeclaration.Members.Add(property);

            return base.VisitSynthesized(synthesizedBuilder);
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ParameterPropertyGenerator));
    }
}
