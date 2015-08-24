using HarshPoint.ShellployGenerator.Builders;
using System;
using System.CodeDom;
using System.Collections.ObjectModel;

namespace HarshPoint.ShellployGenerator.CodeGen
{
    internal sealed class NewObjectAssignmentVisitor : PropertyModelVisitor
    {
        private readonly HarshScopedValue<String> _renamed
            = new HarshScopedValue<String>();

        private readonly HarshScopedValue<Object> _fixedValue
            = new HarshScopedValue<Object>();

        private readonly CodeExpression _targetObject;

        public NewObjectAssignmentVisitor(CodeExpression targetObject)
        {
            if (targetObject == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(targetObject));
            }

            _targetObject = targetObject;
        }

        public Collection<CodeStatement> Statements { get; }
            = new Collection<CodeStatement>();

        protected internal override PropertyModel VisitAssignedTo(
            PropertyModelAssignedTo property
        )
        {
            var lhs = new CodePropertyReferenceExpression(
                _targetObject,
                property.TargetPropertyName
            );

            var rhs = GetAssignedExpression(property);

            Statements.Add(
                new CodeAssignStatement(lhs, rhs)
            );

            return property;
        }

        protected internal override PropertyModel VisitFixed(
            PropertyModelFixed property
        )
        {
            if (property == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(property));
            }

            using (_fixedValue.EnterIfHasNoValue(property.Value))
            {
                return base.VisitFixed(property);
            }
        }

        protected internal override PropertyModel VisitRenamed(
            PropertyModelRenamed property
        )
        {
            if (property == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(property));
            }

            using (_renamed.EnterIfHasNoValue(property.PropertyName))
            {
                return base.VisitRenamed(property);
            }
        }

        private CodeExpression GetAssignedExpression(PropertyModel property)
        {
            if (_fixedValue.HasValue)
            {
                return CodeLiteralExpression.Create(_fixedValue.Value);
            }

            return new CodePropertyReferenceExpression(
                This,
                _renamed.HasValue ? _renamed.Value : property.Identifier
            );
        }

        private static readonly CodeExpression This
            = new CodeThisReferenceExpression();

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(NewObjectAssignmentVisitor));
    }
}
