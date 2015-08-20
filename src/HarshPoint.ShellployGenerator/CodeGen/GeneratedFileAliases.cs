using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace HarshPoint.ShellployGenerator.CodeGen
{
    internal sealed class GeneratedFileAliases : GeneratedFile
    {
        public GeneratedFileAliases(IEnumerable<ShellployCommand> commands)
        {
            Commands = commands;
            FileName = "HarshPoint.Shellploy.psm1";
        }

        public IEnumerable<ShellployCommand> Commands { get; }

        protected override void Write(TextWriter writer)
        {
            var aliases = ImmutableDictionary.CreateRange(
                from cmd in Commands
                from alias in cmd.Aliases
                orderby alias
                select new KeyValuePair<String, ShellployCommand>(
                    alias, cmd
                )
            );

            foreach (var a in aliases)
            {
                writer.WriteLine(
                    $"Set-Alias -Name {a.Key} -Value {a.Value.Name}"
                );
            }

            writer.WriteLine();
            writer.WriteLine("Export-ModuleMember -Alias @(");

            foreach (var a in aliases)
            {
                writer.WriteLine($"    '{a.Key}'");
            }

            writer.WriteLine(")");
        }
    }
}