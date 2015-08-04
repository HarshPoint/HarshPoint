using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace HarshPoint.Linq
{
    internal sealed class HarshExpressionEqualityComparer : IEqualityComparer<Expression>
    {
        private HarshExpressionEqualityComparer() { }

        public Boolean Equals(Expression x, Expression y)
        {
            if (ReferenceEquals(x, y)) return true;
            if ((x == null) || (y == null)) return false;

            if (x.NodeType != y.NodeType) return false;
            if (x.Type != y.Type) return false;

            return Comparer.Equals(x, y);
        }

        public Int32 GetHashCode(Expression obj) => Comparer.GetHashCode(obj);

        public static HarshExpressionEqualityComparer Instance { get; }
            = new HarshExpressionEqualityComparer();

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode")]
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
        private static HarshRecursiveEqualityComparer BuildComparer()
        {
            var comparer = new HarshRecursiveEqualityComparer();

            comparer.AddProperty<Expression>(e => e.CanReduce);
            comparer.AddProperty<Expression>(e => e.NodeType);
            comparer.AddProperty<Expression>(e => e.Type);

            comparer.AddProperty<BinaryExpression>(be => be.Conversion);
            comparer.AddProperty<BinaryExpression>(be => be.IsLifted);
            comparer.AddProperty<BinaryExpression>(be => be.IsLiftedToNull);
            comparer.AddProperty<BinaryExpression>(be => be.Left);
            comparer.AddProperty<BinaryExpression>(be => be.Method);
            comparer.AddProperty<BinaryExpression>(be => be.Right);

            comparer.AddProperty<BlockExpression>(be => be.Expressions);
            comparer.AddProperty<BlockExpression>(be => be.Result);
            comparer.AddProperty<BlockExpression>(be => be.Variables);

            comparer.AddProperty<ConditionalExpression>(ce => ce.IfFalse);
            comparer.AddProperty<ConditionalExpression>(ce => ce.IfTrue);
            comparer.AddProperty<ConditionalExpression>(ce => ce.Test);

            comparer.AddProperty<ConstantExpression>(ce => ce.Value);

            comparer.AddProperty<DebugInfoExpression>(die => die.Document);
            comparer.AddProperty<DebugInfoExpression>(die => die.EndColumn);
            comparer.AddProperty<DebugInfoExpression>(die => die.EndLine);
            comparer.AddProperty<DebugInfoExpression>(die => die.StartColumn);
            comparer.AddProperty<DebugInfoExpression>(die => die.StartLine);

            comparer.AddProperty<SymbolDocumentInfo>(sdi => sdi.DocumentType);
            comparer.AddProperty<SymbolDocumentInfo>(sdi => sdi.FileName);
            comparer.AddProperty<SymbolDocumentInfo>(sdi => sdi.Language);
            comparer.AddProperty<SymbolDocumentInfo>(sdi => sdi.LanguageVendor);

            comparer.AddProperty<DynamicExpression>(de => de.Arguments);
            comparer.AddProperty<DynamicExpression>(de => de.Binder);
            comparer.AddProperty<DynamicExpression>(de => de.DelegateType);

            comparer.AddProperty<GotoExpression>(ge => ge.Kind);
            comparer.AddProperty<GotoExpression>(ge => ge.Target);
            comparer.AddProperty<GotoExpression>(ge => ge.Value);

            comparer.AddProperty<IndexExpression>(ie => ie.Arguments);
            comparer.AddProperty<IndexExpression>(ie => ie.Indexer);
            comparer.AddProperty<IndexExpression>(ie => ie.Object);

            comparer.AddProperty<InvocationExpression>(ie => ie.Arguments);
            comparer.AddProperty<InvocationExpression>(ie => ie.Expression);

            comparer.AddProperty<LabelExpression>(le => le.DefaultValue);
            comparer.AddProperty<LabelExpression>(le => le.Target);

            comparer.AddProperty<LambdaExpression>(le => le.Body);
            comparer.AddProperty<LambdaExpression>(le => le.Name);
            comparer.AddProperty<LambdaExpression>(le => le.Parameters);
            comparer.AddProperty<LambdaExpression>(le => le.ReturnType);
            comparer.AddProperty<LambdaExpression>(le => le.TailCall);

            comparer.AddProperty<ListInitExpression>(lie => lie.Initializers);
            comparer.AddProperty<ListInitExpression>(lie => lie.NewExpression);

            comparer.AddProperty<ElementInit>(ei => ei.AddMethod);
            comparer.AddProperty<ElementInit>(ei => ei.Arguments);

            comparer.AddProperty<LoopExpression>(le => le.Body);
            comparer.AddProperty<LoopExpression>(le => le.BreakLabel);
            comparer.AddProperty<LoopExpression>(le => le.ContinueLabel);

            comparer.AddProperty<MemberExpression>(me => me.Expression);
            comparer.AddProperty<MemberExpression>(me => me.Member);

            comparer.AddProperty<MemberInitExpression>(mie => mie.Bindings);
            comparer.AddProperty<MemberInitExpression>(mie => mie.NewExpression);

            comparer.AddProperty<MemberBinding>(mb => mb.BindingType);
            comparer.AddProperty<MemberBinding>(mb => mb.Member);

            comparer.AddProperty<MethodCallExpression>(mce => mce.Arguments);
            comparer.AddProperty<MethodCallExpression>(mce => mce.Method);
            comparer.AddProperty<MethodCallExpression>(mce => mce.Object);

            comparer.AddProperty<NewArrayExpression>(nae => nae.Expressions);

            comparer.AddProperty<NewExpression>(ne => ne.Arguments);
            comparer.AddProperty<NewExpression>(ne => ne.Constructor);
            comparer.AddProperty<NewExpression>(ne => ne.Members);

            comparer.AddProperty<ParameterExpression>(pe => pe.IsByRef);
            comparer.AddProperty<ParameterExpression>(pe => pe.Name);

            comparer.AddProperty<RuntimeVariablesExpression>(rve => rve.Variables);

            comparer.AddProperty<SwitchExpression>(se => se.Cases);
            comparer.AddProperty<SwitchExpression>(se => se.Comparison);
            comparer.AddProperty<SwitchExpression>(se => se.DefaultBody);
            comparer.AddProperty<SwitchExpression>(se => se.SwitchValue);

            comparer.AddProperty<SwitchCase>(sc => sc.Body);
            comparer.AddProperty<SwitchCase>(sc => sc.TestValues);

            comparer.AddProperty<TryExpression>(te => te.Body);
            comparer.AddProperty<TryExpression>(te => te.Fault);
            comparer.AddProperty<TryExpression>(te => te.Finally);
            comparer.AddProperty<TryExpression>(te => te.Handlers);

            comparer.AddProperty<CatchBlock>(cb => cb.Body);
            comparer.AddProperty<CatchBlock>(cb => cb.Filter);
            comparer.AddProperty<CatchBlock>(cb => cb.Test);
            comparer.AddProperty<CatchBlock>(cb => cb.Variable);

            comparer.AddProperty<TypeBinaryExpression>(tbe => tbe.Expression);
            comparer.AddProperty<TypeBinaryExpression>(tbe => tbe.TypeOperand);

            comparer.AddProperty<UnaryExpression>(ue => ue.IsLifted);
            comparer.AddProperty<UnaryExpression>(ue => ue.IsLiftedToNull);
            comparer.AddProperty<UnaryExpression>(ue => ue.Method);
            comparer.AddProperty<UnaryExpression>(ue => ue.Operand);

            return comparer;
        }

        private static readonly HarshRecursiveEqualityComparer Comparer
            = BuildComparer();
    }
}
