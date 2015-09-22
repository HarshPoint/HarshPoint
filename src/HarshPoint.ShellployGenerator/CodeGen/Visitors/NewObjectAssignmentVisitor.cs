using HarshPoint.ShellployGenerator.Builders;
using System;
using System.CodeDom;
using System.Linq;
using SMA = System.Management.Automation;

namespace HarshPoint.ShellployGenerator.CodeGen
{
    internal sealed class NewObjectAssignmentVisitor : PropertyModelVisitor
    {
        private readonly HarshScopedValue<Object> _fixedValue
            = new HarshScopedValue<Object>();

        private readonly HarshScopedValue<PropertyModelConditionalFixed> _conditionalFixedProperty
            = new HarshScopedValue<PropertyModelConditionalFixed>();

        private readonly HarshScopedValue<PropertyModelNegated> _negated
            = new HarshScopedValue<PropertyModelNegated>();

        private readonly HarshScopedValue<CodeExpression> _lhs
            = new HarshScopedValue<CodeExpression>();

        private readonly CodeExpression _targetObject;

        public NewObjectAssignmentVisitor(CodeExpression targetObject)
        {
            if (targetObject == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(targetObject));
            }

            _targetObject = targetObject;
        }

        public CodeStatementCollection Statements { get; }
            = new CodeStatementCollection();

        protected internal override PropertyModel VisitAssignedTo(
            PropertyModelAssignedTo property
        )
        {
            if (property == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(property));
            }

            var lhs = new CodePropertyReferenceExpression(
                _targetObject,
                property.TargetPropertyName
            );

            using (_lhs.EnterIfHasNoValue(lhs))
            {
                return base.VisitAssignedTo(property);
            }
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

        protected internal override PropertyModel VisitConditionalFixed(
            PropertyModelConditionalFixed property
        )
        {
            if (property == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(property));
            }

            using (_conditionalFixedProperty.EnterIfHasNoValue(property))
            {
                return base.VisitConditionalFixed(property);
            }
        }

        protected internal override PropertyModel VisitNegated(
            PropertyModelNegated property
        )
        {
            using (_negated.EnterIfHasNoValue(property))
            {
                return base.VisitNegated(property);
            }
        }

        protected internal override PropertyModel VisitSynthesized(
            PropertyModelSynthesized property
        )
        {
            if (property == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(property));
            }

            if (_fixedValue.HasValue)
            {
                AddTargetAssignment(
                    property,
                    Statements,
                    CodeLiteralExpression.Create(_fixedValue.Value)
                );
            }
            else if (_conditionalFixedProperty.HasValue)
            {
                var value = _conditionalFixedProperty.Value;
                var builder = new CodeDomElseIfBuilder();

                foreach (var conditionalValue in value.ConditionalValues)
                {
                    builder.ElseIf(
                        conditionalValue.Item1,
                        GetTargetAssignment(
                            property,
                            CodeLiteralExpression.Create(conditionalValue.Item2)
                        )
                    );
                }

                if (value.ElseValue != null)
                {
                    builder.Else(
                        GetTargetAssignment(
                            property,
                            CodeLiteralExpression.Create(value.ElseValue)
                        )
                    );
                }

                var statement = builder.ToStatement();
                if (statement != null)
                {
                    Statements.Add(statement);
                }
            }
            else if (property.PropertyType == typeof(SMA.SwitchParameter))
            {
                var condition = new CodeConditionStatement(
                    IsSwitchPresent(property)
                );

                if (_negated.HasValue)
                {
                    var validate = new CodeConditionStatement(
                        IsSwitchPresent(
                            _negated.Value.PositivePropertyName
                        ),

                        WriteExclusiveSwitchValidationError(
                            property.Identifier,
                            _negated.Value.PositivePropertyName
                        ),

                        new CodeMethodReturnStatement()
                    );

                    condition.TrueStatements.Add(validate);
                }

                AddTargetAssignment(
                    property,
                    condition.TrueStatements,
                    new CodePrimitiveExpression(
                        _negated.HasValue ? false : true
                    )
                );

                Statements.Add(condition);
            }
            else
            {
                AddTargetAssignment(
                    property,
                    Statements,
                    GetPropertyExpression(property)
                );
            }

            return base.VisitSynthesized(property);
        }

        private static CodeStatement WriteExclusiveSwitchValidationError(
            String negativePropertyName,
            String positivePropertyName
        )
            => new CodeExpressionStatement(
                new CodeMethodInvokeExpression(
                    This,
                    "WriteExclusiveSwitchValidationError",
                    new CodePrimitiveExpression(positivePropertyName),
                    new CodePrimitiveExpression(negativePropertyName)
                )
            );

        private void AddTargetAssignment(
            PropertyModel current,
            CodeStatementCollection statements,
            CodeExpression rhs
        )
        {
            var statement = GetTargetAssignment(current, rhs);
            if (statement != null)
            {
                statements.Add(statement);
            }
        }

        private CodeStatement GetTargetAssignment(
            PropertyModel current,
            CodeExpression rhs
        )
        {
            if (_lhs.HasValue)
            {
                return new CodeAssignStatement(
                    _lhs.Value,
                    rhs
                );
            }
            else
            {
                Logger.Information(
                    "Property {Property} is not wrapped with a " +
                    "PropertyModelAssignedTo, no assignment will be " +
                    "generated.",
                    current.Identifier
                );

                return null;
            }
        }

        private CodeExpression GetPropertyExpression(
            PropertyModelSynthesized property
        )
            => GetPropertyExpression(
                RenamedPropertyName ?? property.Identifier
            );

        private CodePropertyReferenceExpression IsSwitchPresent(
            PropertyModelSynthesized property
        )
            => new CodePropertyReferenceExpression(
                GetPropertyExpression(property),
                "IsPresent"
            );

        private static CodePropertyReferenceExpression IsSwitchPresent(
            String propertyName
        )
            => new CodePropertyReferenceExpression(
                GetPropertyExpression(propertyName),
                "IsPresent"
            );

        private static CodeExpression GetPropertyExpression(
            String propertyName
        )
            => new CodePropertyReferenceExpression(This, propertyName);

        private static readonly CodeExpression This
            = new CodeThisReferenceExpression();

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(NewObjectAssignmentVisitor));
    }
}
