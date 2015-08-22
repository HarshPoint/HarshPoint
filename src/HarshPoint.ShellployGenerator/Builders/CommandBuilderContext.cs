using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace HarshPoint.ShellployGenerator.Builders
{
    public sealed class CommandBuilderContext
    {
        private ImmutableList<CommandBuilder> _builders
            = ImmutableList<CommandBuilder>.Empty;

        private ImmutableDictionary<Type, NewObjectCommandBuilder> _newObjectBuilders
            = ImmutableDictionary<Type, NewObjectCommandBuilder>.Empty;

        public void AddBuildersFrom(Assembly assembly)
        {
            var builders =
                from type in assembly.DefinedTypes
                where
                    CommandBuilderTypeInfo.IsAssignableFrom(type) &&
                    !type.ContainsGenericParameters &&
                    !type.IsAbstract

                select CreateBuilder(type);

            _builders = _builders.AddRange(builders);

            _newObjectBuilders = _newObjectBuilders.AddRange(
                builders
                .OfType<NewObjectCommandBuilder>()
                .Select(nocb => HarshKeyValuePair.Create(nocb.TargetType, nocb))
            );
        }

        public void Add(CommandBuilder builder)
        {
            if (builder == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(builder));
            }

            var newObjectBuilder = (builder as NewObjectCommandBuilder);

            if (newObjectBuilder != null)
            {
                _newObjectBuilders = _newObjectBuilders.Add(
                    newObjectBuilder.TargetType,
                    newObjectBuilder
                );
            }

            _builders = _builders.Add(builder);
            builder.Context = this;
        }

        public IEnumerable<CommandBuilder> Builders => _builders;

        public NewObjectCommandBuilder GetNewObjectCommandBuilder(
            Type targetType
        )
        {
            if (targetType == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(targetType));
            }

            var builder = _newObjectBuilders.GetValueOrDefault(targetType);

            if (builder == null)
            {
                throw Logger.Fatal.ArgumentFormat(
                    nameof(targetType),
                    SR.CommandBuilderContext_NoBuilder,
                    targetType
                );
            }

            return builder;
        }

        private CommandBuilder CreateBuilder(Type builderType)
        {
            var result = (CommandBuilder)Activator.CreateInstance(builderType);
            result.Context = this;
            return result;
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(CommandBuilderContext));

        private static readonly TypeInfo CommandBuilderTypeInfo
            = typeof(CommandBuilder).GetTypeInfo();

        private static readonly TypeInfo NewObjectCommandBuilderTypeInfo
            = typeof(NewObjectCommandBuilder).GetTypeInfo();
    }
}
