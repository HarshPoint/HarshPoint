using HarshPoint.ShellployGenerator.Builders;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;

using static System.FormattableString;

namespace HarshPoint.ShellployGenerator.CodeGen
{
    internal sealed class DeclarePropertiesVisitor : PropertyModelVisitor
    {
        private readonly HarshScopedValue<Object> _defaultValue
            = new HarshScopedValue<Object>();

        private readonly HarshScopedValue<String> _renaming
            = new HarshScopedValue<String>();

        public DeclarePropertiesVisitor(CodeTypeDeclaration typeDeclaration)
        {
            if (typeDeclaration == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(typeDeclaration));
            }

            TypeDeclaration = typeDeclaration;
        }

        public CodeTypeDeclaration TypeDeclaration { get; }

        protected internal override PropertyModel VisitDefaultValue(
            PropertyModelDefaultValue propertyModel
        )
        {
            if (propertyModel == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(propertyModel));
            }

            using (_defaultValue.EnterIfHasNoValue(propertyModel.DefaultValue))
            {
                return base.VisitDefaultValue(propertyModel);
            }
        }

        protected internal override PropertyModel VisitFixed(
            PropertyModelFixed propertyModel
        )
            => null; // do not generate properties for fixed parameters

        protected internal override PropertyModel VisitIgnored(
            PropertyModelIgnored propertyModel
        )
            // do not generate properties for ignored parameters
            // they should've been removed by RemoveIgnoredOrUnsythesized
            // anyway
            => null;

        protected internal override PropertyModel VisitRenamed(
            PropertyModelRenamed propertyModel
        )
        {
            if (propertyModel == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(propertyModel));
            }

            using (_renaming.EnterIfHasNoValue(propertyModel.PropertyName))
            {
                return base.VisitRenamed(propertyModel);
            }
        }

        protected internal override PropertyModel VisitSynthesized(
            PropertyModelSynthesized propertyModel
        )
        {
            if (propertyModel == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(propertyModel));
            }

            var name = _renaming.Value ?? propertyModel.Identifier;
            var type = new CodeTypeReference(propertyModel.PropertyType);

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
                propertyModel.Attributes
            );

            TypeDeclaration.Members.Add(codeField);
            TypeDeclaration.Members.Add(codeProperty);

            return base.VisitSynthesized(propertyModel);
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
                throw Logger.Fatal.InvalidOperationFormat(
                    SR.CodeDomExtensions_FieldExists,
                    name
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

        private static CodeMemberProperty CreateProperty(
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

            return Invariant(
                $"_{Char.ToLowerInvariant(value[0])}{value.Substring(1)}"
            );
        }

        private const MemberAttributes PublicFinal =
            MemberAttributes.Public |
            MemberAttributes.Final;

        private static readonly CodeExpression This =
            new CodeThisReferenceExpression();

        private static readonly CodeExpression PropertySetValue =
            new CodePropertySetValueReferenceExpression();

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(DeclarePropertiesVisitor));
    }
}
