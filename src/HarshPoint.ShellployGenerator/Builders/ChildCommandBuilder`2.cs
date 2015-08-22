using System;
using System.Linq.Expressions;

namespace HarshPoint.ShellployGenerator.Builders
{
    public sealed class ChildCommandBuilder<TProvisioner, TParent> :
        IChildCommandBuilder
    {
        private readonly PropertyModelContainer _parameterBuilders
            = new PropertyModelContainer();

        internal ChildCommandBuilder()
        {
            _parameterBuilders.Update(
                CommandBuilder.InputObjectIdentifier,
                new PropertyModelIgnored()
            );
        }

        public IChildParameterBuilder<TParent> Parameter(
            Expression<Func<TParent, Object>> expression
        )
            => _parameterBuilders.GetParameterBuilder(expression);

        public IChildParameterBuilder Parameter(
            String name
        )
            => _parameterBuilders.GetParameterBuilder(name);

        PropertyModelContainer IChildCommandBuilder.ParameterBuilders
            => _parameterBuilders;

        Type IChildCommandBuilder.ParentType => typeof(TParent);

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ChildCommandBuilder<,>));
    }
}