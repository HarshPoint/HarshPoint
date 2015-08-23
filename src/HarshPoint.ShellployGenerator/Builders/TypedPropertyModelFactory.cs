using System;
using System.Linq.Expressions;

namespace HarshPoint.ShellployGenerator.Builders
{
    internal static class TypedPropertyModelFactory
    {
        public static ParameterBuilder<TTarget> Parameter<TTarget>(
            INewObjectCommandBuilder<TTarget> builder,
            Expression<Func<TTarget, Object>> expression
        )
        {
            if (builder == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(builder));
            }

            if (expression == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(expression));
            }

            return builder.PropertyContainer.GetParameterBuilder(expression);
        }

        public static ParameterBuilder<TTarget> PositionalParameter<TTarget>(
            INewObjectCommandBuilder<TTarget> builder,
            Expression<Func<TTarget, Object>> expression
        )
        {
            if (builder == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(builder));
            }

            if (expression == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(expression));
            }

            return builder.PropertyContainer.GetParameterBuilder(
                expression, 
                isPositional: true
            );
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(TypedPropertyModelFactory));
    }
}
