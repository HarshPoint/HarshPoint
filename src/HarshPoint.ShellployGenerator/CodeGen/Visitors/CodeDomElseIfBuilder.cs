using System;
using System.CodeDom;

namespace HarshPoint.ShellployGenerator.CodeGen
{
    public sealed class CodeDomElseIfBuilder
    {
        private CodeConditionStatement _outerCondition;
        private CodeConditionStatement _innerCondition;

        public void ElseIf(
            CodeExpression conditionExpression,
            params CodeStatement[] trueStatements
        )
        {
            if (conditionExpression == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(conditionExpression));
            }

            if (trueStatements == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(trueStatements));
            }

            if (_innerCondition != null && _innerCondition.FalseStatements.Any())
            {
                throw new InvalidOperationException(SR.CodeDomElseIfBuilder_ElseCalled);
            }

            var condition = new CodeConditionStatement(conditionExpression);
            condition.TrueStatements.AddRange(trueStatements);

            if (_innerCondition != null)
            {
                _innerCondition.FalseStatements.Add(condition);
            }
            else
            {
                _outerCondition = condition;
            }
            _innerCondition = condition;
        }

        public void Else(params CodeStatement[] falseStatements)
        {
            if (falseStatements == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(falseStatements));
            }

            if (_innerCondition == null)
            {
                throw Logger.Fatal.InvalidOperation(SR.CodeDomElseIfBuilder_OnlyElse);
            }
            _innerCondition.FalseStatements.AddRange(falseStatements);
        }

        public CodeStatement ToStatement()
            => _outerCondition;

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(CodeDomElseIfBuilder));
    }
}