using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace HarshPoint.ShellployGenerator
{
    internal static class ShellployMetadata
    {
        public static IEnumerable<ShellployCommand> GetCommands(Assembly assembly)
        {
            var builders = assembly.DefinedTypes
                .Where(ICommandBuilderTypeInfo.IsAssignableFrom)
                .Where(type => !type.IsAbstract && !type.ContainsGenericParameters)
                .Select(Activator.CreateInstance)
                .Cast<IShellployCommandBuilder>()
                .ToImmutableDictionary(
                    builder => builder.ProvisionerType
                );

            return builders.Values.Select(builder => builder.ToCommand(builders));
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ShellployMetadata));

        private static readonly TypeInfo ICommandBuilderTypeInfo
            = typeof(IShellployCommandBuilder).GetTypeInfo();
    }
}
