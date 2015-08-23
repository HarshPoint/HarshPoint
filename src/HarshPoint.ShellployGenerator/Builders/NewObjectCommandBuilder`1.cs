using HarshPoint.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SMA = System.Management.Automation;

namespace HarshPoint.ShellployGenerator.Builders
{
    public class NewObjectCommandBuilder<TTarget> : NewObjectCommandBuilder
    {
        public NewObjectCommandBuilder()
            : this(new HarshParameterizedObjectMetadata(typeof(TTarget)))
        {
        }

        public NewObjectCommandBuilder(
            HarshParameterizedObjectMetadata metadata
        )
            : base(metadata)
        {
        }

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
                    ChildBuilder.ParentType
                );
            }

            if (action != null)
            {
                action(builder);
            }
        }

        public ParameterBuilder<TTarget> Parameter(
            Expression<Func<TTarget, Object>> expression
        )
        {
            if (expression == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(expression));
            }

            return PropertyContainer.GetParameterBuilder(expression);
        }

        public ParameterBuilder<TTarget> PositionalParameter(
            Expression<Func<TTarget, Object>> expression
        )
        {
            if (expression == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(expression));
            }

            return PropertyContainer.GetParameterBuilder(
                expression, 
                isPositional: true
            );
        }

        protected override IEnumerable<PropertyModel> CreatePropertiesLocal()
            => SetValueFromPipelineByPropertyName.Visit(
                base.CreatePropertiesLocal()
            );

        private static readonly PropertyModelVisitor SetValueFromPipelineByPropertyName
            = new AttributeNamedArgumentVisitor(
                typeof(SMA.ParameterAttribute),
                "ValueFromPipelineByPropertyName",
                true
            );

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(NewObjectCommandBuilder<>));
    }
}
