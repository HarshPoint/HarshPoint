using HarshPoint.Provisioning.Implementation;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace HarshPoint.ShellployGenerator.Builders
{

    public class NewProvisionerCommandBuilder<TProvisioner> :
        NewProvisionerCommandBuilder,
        INewObjectCommandBuilder<TProvisioner>
        where TProvisioner : HarshProvisionerBase
    {
        public NewProvisionerCommandBuilder() : base(Metadata) { }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public void AsChildOf<TParent>()
        {
            AsChildOf<TParent>(null);
        }

        public void AsChildOf<TParent>(
            Action<ChildCommandBuilder<TProvisioner, TParent>> action
        )
        {
            if (ChildBuilder == null)
            {
                ChildBuilder = new ChildCommandBuilder<TProvisioner, TParent>(
                    this
                );
            }

            var builder = (ChildBuilder as ChildCommandBuilder<TProvisioner, TParent>);

            if (builder == null)
            {
                throw Logger.Fatal.InvalidOperationFormat(
                    SR.CommandBuilder_AlreadyChildOf,
                    ChildBuilder.ParentType
                );
            }

            if (action != null)
            {
                action(builder);
            }
        }

        public ParameterBuilder<TProvisioner> Parameter(
            Expression<Func<TProvisioner, Object>> expression
        )
            => TypedPropertyModelFactory.Parameter(this, expression);

        public ParameterBuilder<TProvisioner> PositionalParameter(
            Expression<Func<TProvisioner, Object>> expression
        )
            => TypedPropertyModelFactory.PositionalParameter(
                this, 
                expression
            );

        PropertyModelContainer INewObjectCommandBuilder<TProvisioner>.PropertyContainer
            => PropertyContainer;

        private static readonly HarshProvisionerMetadata Metadata
           = HarshProvisionerMetadataRepository.Get(typeof(TProvisioner));

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(NewProvisionerCommandBuilder<>));
    }
}