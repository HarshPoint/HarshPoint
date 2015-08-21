using System;
using System.Linq.Expressions;

namespace HarshPoint.ShellployGenerator.Builders
{
    public sealed class ChildCommandBuilder<TProvisioner, TParent> :
        IChildCommandBuilder
    {
        private readonly ParameterBuilderContainer<TParent> _parameters
            = new ParameterBuilderContainer<TParent>();

        internal ChildCommandBuilder()
        {
            _parameters.Update(
                ShellployCommand.InputObjectPropertyName,
                new ParameterBuilderIgnored()
            );
        }

        public IChildParameterBuilderFactory<TParent> Parameter(
            Expression<Func<TParent, Object>> expression
        )
            => _parameters.GetFactory(expression);

        public IChildParameterBuilderFactory<TParent> Parameter(
            String name
        )
            => _parameters.GetFactory(name);

        ParameterBuilderContainer IChildCommandBuilder.Parameters
            => _parameters;

        Type IChildCommandBuilder.ProvisionerType => typeof(TParent);

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ChildCommandBuilder<,>));
    }
}