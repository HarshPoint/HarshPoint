using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace HarshPoint.ShellployGenerator.Builders
{
    internal sealed class CommandBuilderContext
    {
        private ImmutableDictionary<Type, ICommandBuilder> _builders
            = ImmutableDictionary<Type, ICommandBuilder>.Empty;

        public void AddBuildersFrom(Assembly assembly)
        {
            _builders = _builders.AddRange(
                from type in assembly.DefinedTypes
                where
                    ICommandBuilderTypeInfo.IsAssignableFrom(type) &&
                    !type.ContainsGenericParameters &&
                    !type.IsAbstract

                let instance = (ICommandBuilder)Activator.CreateInstance(type)
                select new KeyValuePair<Type, ICommandBuilder>(
                    instance.ProvisionerType,
                    instance
                )
            );
        }

        public IEnumerable<ICommandBuilder> Builders => _builders.Values;

        public ICommandBuilder GetBuilder(Type provisionerType)
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

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(CommandBuilderContext));

        private static readonly TypeInfo ICommandBuilderTypeInfo
            = typeof(ICommandBuilder).GetTypeInfo();
    }
}
