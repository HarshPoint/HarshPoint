using HarshPoint.ObjectModel;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using SMA = System.Management.Automation;

namespace HarshPoint.ShellployGenerator.Builders
{
    public class NewObjectCommandBuilder<TTarget> :
        CommandBuilder,
        INewObjectCommandBuilder
    {
        public NewObjectCommandBuilder()
            : this(new HarshParametrizedObjectMetadata(typeof(TTarget)))
        {
        }

        public NewObjectCommandBuilder(
            HarshParametrizedObjectMetadata metadata
        )
        {
            Attributes.Add(new AttributeData(typeof(SMA.OutputTypeAttribute))
            {
                ConstructorArguments = { TargetType }
            });

            Noun = typeof(TTarget).Name;
            Verb = SMA.VerbsCommon.New;

            if (metadata != null)
            {
                InitializeFromMetadata(metadata);
            }
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

        public ParameterBuilderFactory<TTarget> Parameter(
            Expression<Func<TTarget, Object>> expression
        )
        {
            if (expression == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(expression));
            }

            return ParameterBuilders.GetFactory(expression);
        }

        public ParameterBuilderFactory<TTarget> PositionalParameter(
            Expression<Func<TTarget, Object>> expression
        )
        {
            if (expression == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(expression));
            }

            return ParameterBuilders.GetFactory(expression, isPositional: true);
        }

        public override ShellployCommand ToCommand()
        {
            var result = base.ToCommand();
            result.ParentProvisionerTypes = ParentTargetTypes;
            result.ProvisionerType = TargetType;
            return result;
        }

        protected override IEnumerable<ShellployCommandProperty> CreateProperties()
            => ((INewObjectCommandBuilder)(this))
                .GetParametersRecursively()
                .SelectMany(p => p.Value.Synthesize());

        public Type TargetType => typeof(TTarget);


        private void InitializeFromMetadata(HarshParametrizedObjectMetadata metadata)
        {
            if ((metadata.DefaultParameterSet != null) &&
                (!metadata.DefaultParameterSet.IsImplicit))
            {
                DefaultParameterSetName = metadata.DefaultParameterSet.Name;
            }

            foreach (var grouping in metadata.PropertyParameters)
            {
                var property = grouping.Key;
                var parameters = grouping.AsEnumerable();

                ValidateParameterName(property.Name);

                if (parameters.Any(p => p.IsCommonParameter))
                {
                    parameters = parameters.Take(1);
                }

                var attributes = parameters.Select(CreateParameterAttribute);

                var synthesized = new ParameterBuilderSynthesized(
                    property.Name,
                    property.PropertyType,
                    metadata.ObjectType,
                    attributes
                );

                ParameterBuilders.Update(property.Name, synthesized);
            }
        }

        private ImmutableList<KeyValuePair<String, ParameterBuilder>>
        GetParametersRecursively()
        {
            var parametersSorted
                = ImmutableList<KeyValuePair<String, ParameterBuilder>>.Empty;

            if (ChildBuilder != null)
            {
                parametersSorted = ChildBuilder.ParameterBuilders
                    .ApplyTo(ParentBuilder.GetParametersRecursively())
                    .ToImmutableList();
            }

            parametersSorted = parametersSorted.AddRange(
                from param in ParameterBuilders
                select param.WithValue(
                    SetValueFromPipelineByPropertyName(param.Value)
                )
            );

            return parametersSorted;
        }

        private IChildCommandBuilder ChildBuilder { get; set; }

        private INewObjectCommandBuilder ParentBuilder
        {
            get
            {
                if (ChildBuilder != null)
                {
                    return Context.GetNewObjectCommandBuilder(ChildBuilder.ParentType);
                }

                return null;
            }
        }

        private IImmutableList<Type> ParentTargetTypes
        {
            get
            {
                if (ParentBuilder != null)
                {
                    return ParentBuilder
                        .ParentTargetTypes
                        .Add(ParentBuilder.TargetType);
                }

                return ImmutableList<Type>.Empty;
            }
        }

        IImmutableList<Type> INewObjectCommandBuilder.ParentTargetTypes
            => ParentTargetTypes;

        ImmutableList<KeyValuePair<String, ParameterBuilder>>
        INewObjectCommandBuilder.GetParametersRecursively()
            => GetParametersRecursively();

        private static AttributeData CreateParameterAttribute(Parameter param)
        {
            var data = new AttributeData(typeof(SMA.ParameterAttribute));

            if (param.IsMandatory)
            {
                data.NamedArguments["Mandatory"] = true;
            }

            if (!param.IsCommonParameter)
            {
                data.NamedArguments["ParameterSetName"] = param.ParameterSetName;
            }

            return data;
        }

        private static ParameterBuilder SetValueFromPipelineByPropertyName(
            ParameterBuilder parameter
        )
            => new ParameterBuilderAttributeNamedArgument(
                typeof(SMA.ParameterAttribute),
                "ValueFromPipelineByPropertyName",
                true,
                parameter
            );

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(NewObjectCommandBuilder<>));
    }
}
