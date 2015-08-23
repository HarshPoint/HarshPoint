using System;
using System.Linq.Expressions;

namespace HarshPoint.ShellployGenerator.Builders
{
    public sealed class ChildCommandBuilder<TProvisioner, TParent> :
        IChildProvisionerCommandBuilder
    {
        private readonly PropertyModelContainer _parameterBuilders;

        internal ChildCommandBuilder(NewProvisionerCommandBuilder owner)
        {
            _parameterBuilders = new PropertyModelContainer(owner);

            _parameterBuilders.Update(
                NewProvisionerCommandBuilder.InputObjectName,
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

        PropertyModelContainer IChildProvisionerCommandBuilder.PropertyContainer
            => _parameterBuilders;

        Type IChildProvisionerCommandBuilder.ParentType => typeof(TParent);

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ChildCommandBuilder<,>));
    }
}