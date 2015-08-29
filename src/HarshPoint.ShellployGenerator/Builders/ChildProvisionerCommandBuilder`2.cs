using System;
using System.Linq.Expressions;

namespace HarshPoint.ShellployGenerator.Builders
{
    public sealed class ChildCommandBuilder<TProvisioner, TParent> :
        IChildProvisionerCommandBuilder
    {
        private readonly NewProvisionerCommandBuilder _owner;
        private readonly PropertyModelContainer _parameterBuilders;

        internal ChildCommandBuilder(NewProvisionerCommandBuilder owner)
        {
            if (owner == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(owner));
            }

            _owner = owner;
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

        NewProvisionerCommandBuilder IChildProvisionerCommandBuilder.ParentBuilder
            => (NewProvisionerCommandBuilder)_owner.Context
                .GetNewProvisionerCommandBuilder(typeof(TParent));

        PropertyModelContainer IChildProvisionerCommandBuilder.PropertyContainer
            => _parameterBuilders;

        Type IChildProvisionerCommandBuilder.ParentType => typeof(TParent);

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ChildCommandBuilder<,>));
    }
}