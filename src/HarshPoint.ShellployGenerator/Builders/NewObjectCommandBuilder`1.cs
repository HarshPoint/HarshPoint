using HarshPoint.ObjectModel;
using System;
using System.Linq.Expressions;

namespace HarshPoint.ShellployGenerator.Builders
{
    public class NewObjectCommandBuilder<TTarget> : 
        NewObjectCommandBuilder,
        INewObjectCommandBuilder<TTarget>
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

        public ParameterBuilder<TTarget> Parameter(
            Expression<Func<TTarget, Object>> expression
        )
            => TypedPropertyModelFactory.Parameter(this, expression);

        public ParameterBuilder<TTarget> PositionalParameter(
            Expression<Func<TTarget, Object>> expression
        )
            => TypedPropertyModelFactory.PositionalParameter(this, expression);

        PropertyModelContainer INewObjectCommandBuilder<TTarget>.PropertyContainer
            => PropertyContainer;
    }
}
