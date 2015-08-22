using System;
using System.Linq.Expressions;

namespace HarshPoint.ShellployGenerator.Builders
{
    public sealed class ChildCommandBuilder<TProvisioner, TParent> :
        IChildCommandBuilder
    {
        private readonly ParameterBuilderContainer _parameterBuilders
            = new ParameterBuilderContainer();

        internal ChildCommandBuilder()
        {
            _parameterBuilders.Update(
                CommandBuilder.InputObjectPropertyName,
                new ParameterBuilderIgnored()
            );
        }

        public IChildParameterBuilderFactory<TParent> Parameter(
            Expression<Func<TParent, Object>> expression
        )
            => _parameterBuilders.GetFactory(expression);

        public IChildParameterBuilderFactory Parameter(
            String name
        )
            => _parameterBuilders.GetFactory(name);

        ParameterBuilderContainer IChildCommandBuilder.ParameterBuilders
            => _parameterBuilders;

        Type IChildCommandBuilder.ParentType => typeof(TParent);

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ChildCommandBuilder<,>));
    }
}