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
    internal partial class CommandBuilder<TProvisioner> : CommandBuilder
    {
        private Int32 _nextPositionalParam;

        public CommandBuilder() : base(SharedMetadata) { }

        public override Type ProvisionerType => typeof(TProvisioner);

        public void AsChildOf<TParent>(
            Action<ChildCommandBuilder<TProvisioner, TParent>> action
        )
        {
            var result = new ChildCommandBuilder<TProvisioner, TParent>();
            ChildBuilder = result;
            action(result);
        }

        public ParameterBuilderFactory<TProvisioner> Parameter(
            Expression<Func<TProvisioner, Object>> expression
        )
            => GetParameterFactory(expression);

        public ParameterBuilderFactory<TProvisioner> PositionalParameter(
            Expression<Func<TProvisioner, Object>> expression
        )
            => GetParameterFactory(expression, isPositional: true);

        public ParameterBuilderFactory<TProvisioner> Parameter(String name)
            => GetParameterFactory(name);

        public ParameterBuilderFactory<TProvisioner> PositionalParameter(
            String name
        )
            => GetParameterFactory(name, isPositional: true);

        private ParameterBuilderFactory<TProvisioner> GetParameterFactory(
            Expression<Func<TProvisioner, Object>> expression,
            Boolean isPositional = false
        )
        {
            if (expression == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(expression));
            }

            var name = expression.ExtractLastPropertyAccess().Name;
            return GetParameterFactory(name, isPositional);
        }

        private ParameterBuilderFactory<TProvisioner> GetParameterFactory(
            String name,
            Boolean isPositional = false
        )
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw Logger.Fatal.ArgumentNullOrWhiteSpace(nameof(name));
            }

            if (isPositional)
            {
                SetParameter(
                    name,
                    new ParameterBuilderPositional(_nextPositionalParam)
                );

                _nextPositionalParam++;
            }

            return new ParameterBuilderFactory<TProvisioner>(
                this,
                name
            );
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(CommandBuilder<>));

        private static readonly HarshProvisionerMetadata SharedMetadata
           = HarshProvisionerMetadataRepository.Get(typeof(TProvisioner));
    }
}