using HarshPoint.ObjectModel;
using HarshPoint.Provisioning.Implementation;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using SMA = System.Management.Automation;

namespace HarshPoint.ShellployGenerator.Builders
{
    internal partial class CommandBuilder<TTarget> : CommandBuilder
    {
        private readonly ParameterBuilderContainer<TTarget> _parameters
            = new ParameterBuilderContainer<TTarget>();

        public CommandBuilder() : base(Metadata) { }

        public override Type ProvisionerType => typeof(TTarget);

        public void AsChildOf<TParent>()
        {
            AsChildOf<TParent>(null);
        }

        public void AsChildOf<TParent>(
            Action<ChildCommandBuilder<TTarget, TParent>> action
        )
        {
            if (ChildBuilder == null)
            {
                ChildBuilder = new ChildCommandBuilder<TTarget, TParent>();
            }

            var builder = (ChildBuilder as ChildCommandBuilder<TTarget, TParent>);
            if (builder == null)
            {
                throw Logger.Fatal.InvalidOperationFormat(
                    SR.CommandBuilder_AlreadyChildOf,
                    ChildBuilder.ProvisionerType
                );
            }

            if (action != null)
            {
                action(builder);
            }
        }

        public ParameterBuilderFactory<TTarget> Parameter(
            Expression<Func<TTarget, Object>> expression
        )
            => _parameters.GetFactory(expression);

        public ParameterBuilderFactory<TTarget> PositionalParameter(
            Expression<Func<TTarget, Object>> expression
        )
            => _parameters.GetFactory(expression, isPositional: true);

        public ParameterBuilderFactory<TTarget> Parameter(String name)
            => _parameters.GetFactory(name);

        public ParameterBuilderFactory<TTarget> PositionalParameter(
            String name
        )
            => _parameters.GetFactory(name, isPositional: true);

        protected sealed override ParameterBuilderContainer Parameters
            => _parameters;


        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(CommandBuilder<>));

        private static readonly HarshProvisionerMetadata Metadata
           = HarshProvisionerMetadataRepository.Get(typeof(TTarget));
    }
}