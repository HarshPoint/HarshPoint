using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace HarshPoint.ShellployGenerator.Builders
{
    internal sealed class CommandBuilderContext
    {
        private ImmutableDictionary<Type, CommandBuilder> _builders
            = ImmutableDictionary<Type, CommandBuilder>.Empty;

        public void AddBuildersFrom(Assembly assembly)
        {
            _builders = _builders.AddRange(
                from type in assembly.DefinedTypes
                where
                    CommandBuilderTypeInfo.IsAssignableFrom(type) &&
                    !type.ContainsGenericParameters &&
                    !type.IsAbstract

                let instance = CreateBuilder(type)
                
                select new KeyValuePair<Type, CommandBuilder>(
                    instance.ProvisionerType,
                    instance
                )
            );
        }

        public void Add(CommandBuilder builder)
        {
            if (builder == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(builder));
            }

            builder.Context = this;
            _builders = _builders.Add(builder.ProvisionerType, builder);
        }

        public IEnumerable<CommandBuilder> Builders => _builders.Values;

        public CommandBuilder GetBuilder(Type provisionerType)
        {
            if (provisionerType == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(provisionerType));
            }

            var builder = _builders.GetValueOrDefault(provisionerType);

            if (builder == null)
            {
                throw Logger.Fatal.ArgumentFormat(
                    nameof(provisionerType),
                    SR.CommandBuilderContext_NoBuilder,
                    provisionerType
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
