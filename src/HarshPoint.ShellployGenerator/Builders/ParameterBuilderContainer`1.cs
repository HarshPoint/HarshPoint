using System;
using System.Linq.Expressions;

namespace HarshPoint.ShellployGenerator.Builders
{
    internal sealed class ParameterBuilderContainer<TTarget> :
        ParameterBuilderContainer
    {
        private Int32 _nextPositionalParam;


        public ParameterBuilderFactory<TTarget> GetFactory(
            Expression<Func<TTarget, Object>> expression,
            Boolean isPositional = false
        )
        {
            if (expression == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(expression));
            }

            var name = expression.ExtractLastPropertyAccess().Name;
            return GetFactory(name, isPositional);
        }

        public ParameterBuilderFactory<TTarget> GetFactory(
            String name,
            Boolean isPositional = false
        )
        {
            CommandBuilder.ValidateParameterName(name);

            if (isPositional)
            {
                Update(
                    name,
                    new ParameterBuilderPositional(_nextPositionalParam)
                );

                _nextPositionalParam++;
            }

            return new ParameterBuilderFactory<TTarget>(
                this,
                name
            );
        }


        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ParameterBuilderContainer<>));
    }
}
