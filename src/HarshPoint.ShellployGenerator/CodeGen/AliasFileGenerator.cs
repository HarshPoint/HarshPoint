using HarshPoint.ShellployGenerator.Builders;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace HarshPoint.ShellployGenerator.CodeGen
{
    internal sealed class AliasFileGenerator : FileGenerator
    {
        public AliasFileGenerator(IEnumerable<CommandModel> commands)
        {
            Commands = commands;
            FileName = "HarshPoint.Shellploy.psm1";
        }

        public IEnumerable<CommandModel> Commands { get; }

        protected override void Write(TextWriter writer)
        {
            var aliases = ImmutableDictionary.CreateRange(
                from cmd in Commands
                from alias in cmd.Aliases
                orderby alias
                select HarshKeyValuePair.Create(
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