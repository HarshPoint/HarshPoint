using HarshPoint.ShellployGenerator.Builders;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;

namespace HarshPoint.ShellployGenerator.CodeGen
{
    internal sealed class PropertyCodeGenerator : PropertyModelVisitor
    {
        private readonly HarshScopedValue<Object> _defaultValue
            = new HarshScopedValue<Object>();

        private readonly HarshScopedValue<String> _renaming
            = new HarshScopedValue<String>();

        public PropertyCodeGenerator(CodeTypeDeclaration typeDeclaration)
        {
            if (typeDeclaration == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(typeDeclaration));
            }

            TypeDeclaration = typeDeclaration;
        }

        public CodeTypeDeclaration TypeDeclaration { get; }

        protected internal override PropertyModel VisitDefaultValue(
            PropertyModelDefaultValue property
        )
        {
            using (_defaultValue.EnterIfDefault(property.DefaultValue))
            {
                return base.VisitDefaultValue(property);
            }
        }

        protected internal override PropertyModel VisitFixed(
            PropertyModelFixed property
        )
            => null; // do not generate properties for fixed parameters

        protected internal override PropertyModel VisitIgnored(
            PropertyModelIgnored property
        )
            // do not generate properties for ignored parameters
            // they should've been removed by RemoveIgnoredOrUnsythesized
            // anyway
            => null;

        protected internal override PropertyModel VisitRenamed(
            PropertyModelRenamed property
        )
        {
            using (_renaming.EnterIfDefault(property.PropertyName))
            {
                return base.VisitRenamed(property);
            }
        }

        protected internal override PropertyModel VisitSynthesized(
            PropertyModelSynthesized property
        )
        {
            var name = _renaming.Value ?? property.Identifier;
            var type = new CodeTypeReference(property.PropertyType);

            var fieldName = GetFieldName(name);

            var codeField = CreateBackingField(
                fieldName,
                type,
                _defaultValue.Value
            );

            var codeProperty = CreateProperty(
                name,
                type,
                codeField,
                property.Attributes
            );

            TypeDeclaration.Members.Add(codeField);
            TypeDeclaration.Members.Add(codeProperty);

            return base.VisitSynthesized(property);
        }

        private Boolean HasMember(String name)
            => TypeDeclaration.Members
                .Cast<CodeTypeMember>()
                .Any(member => member.Name == name);

        private CodeMemberField CreateBackingField(
            String name,
            CodeTypeReference type,
            Object initExpression)
        {
            if (HasMember(name))
            {
                throw Logger.Fatal.InvalidOperation(
                    SR.CodeDomExtensions_FieldExists
                );
            }

            var codeField = new CodeMemberField()
            {
                Attributes = MemberAttributes.Private,
                Name = name,
                Type = type,
            };

            if (_defaultValue.Value != null)
            {
                codeField.InitExpression = CodeLiteralExpression.Create(
                    initExpression
                );
            }

            return codeField;
        }

        private CodeMemberProperty CreateProperty(
            String name,
            CodeTypeReference type,
            CodeMemberField backingField,
            IEnumerable<AttributeModel> attributes
        )
        {
            var backingFieldRef = new CodeFieldReferenceExpression(
                This,
                backingField.Name
            );

            return new CodeMemberProperty()
            {
                Attributes = PublicFinal,
                CustomAttributes = attributes.ToCodeAttributeDeclarations(),
                Name = name,
                Type = type,

                HasGet = true,
                GetStatements =
                {
                    new CodeMethodReturnStatement(backingFieldRef)
                },

                HasSet = true,
                SetStatements =
                {
                    new CodeAssignStatement(
                        backingFieldRef,
                        PropertySetValue
                    )
                }
            };
        }

        private static String GetFieldName(String value)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                throw Logger.Fatal.ArgumentNullOrWhiteSpace(value);
            }

            return $"_{Char.ToLowerInvariant(value[0])}{value.Substring(1)}";
        }

        private const MemberAttributes PublicFinal =
            MemberAttributes.Public |
            MemberAttributes.Final;

        private static readonly CodeExpression This =
            new CodeThisReferenceExpression();

        private static readonly CodeExpression PropertySetValue =
            new CodePropertySetValueReferenceExpression();

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(PropertyCodeGenerator));
    }
}
