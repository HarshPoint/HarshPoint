using HarshPoint.Provisioning.Implementation;
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

        private ImmutableDictionary<Type, NewProvisionerCommandBuilder> _provisionerBuilders
            = ImmutableDictionary<Type, NewProvisionerCommandBuilder>.Empty;

        public void AddBuildersFrom(Assembly assembly)
        {
            if (assembly == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(assembly));
            }

            var builderTypes =
                from type in assembly.DefinedTypes
                where
                    CommandBuilderTypeInfo.IsAssignableFrom(type) &&
                    !type.ContainsGenericParameters &&
                    !type.IsAbstract &&
                    type.DeclaredConstructors.Any(
                        ctor => !ctor.IsStatic && !ctor.GetParameters().Any()
                    )

                select type;

            var builders = builderTypes
                .Select(CreateBuilder)
                .ToArray();

            _builders = _builders.AddRange(builders);

            _provisionerBuilders = _provisionerBuilders.AddRange(
                builders
                .OfType<NewProvisionerCommandBuilder>()
                .Select(npcb => HarshKeyValuePair.Create(npcb.TargetType, npcb))
            );
        }

        public void Add(CommandBuilder builder)
        {
            if (builder == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(builder));
            }

            var provisionerBuilder = (builder as NewProvisionerCommandBuilder);

            if (provisionerBuilder != null)
            {
                _provisionerBuilders = _provisionerBuilders.Add(
                    provisionerBuilder.TargetType,
                    provisionerBuilder
                );
            }

            _builders = _builders.Add(builder);
            builder.Context = this;
        }

        public IEnumerable<CommandBuilder> Builders => _builders;

        public NewObjectCommandBuilder GetNewProvisionerCommandBuilder(
            Type targetType
        )
        {
            if (targetType == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(targetType));
            }

            var builder = _provisionerBuilders.GetValueOrDefault(targetType);

            if (builder == null)
            {
                builder = new NewProvisionerCommandBuilder(
                    HarshProvisionerMetadataRepository.Get(targetType)
                );

                _builders = _builders.Add(builder);
                _provisionerBuilders = _provisionerBuilders.Add(
                    targetType,
                    builder
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
    }
}
