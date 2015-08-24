using HarshPoint.ShellployGenerator.Builders;
using System;
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
            var aliases = ImmutableArray.CreateRange(
                from cmd in Commands
                from alias in cmd.Aliases
                orderby alias
                select Tuple.Create(alias, cmd)
            );

            foreach (var a in aliases.OrderBy(a => a.Item1))
            {
                writer.WriteLine(
                    $"Set-Alias -Name {a.Item1} -Value {a.Item2.Name}"
                );
            }

            writer.WriteLine();
            writer.WriteLine("Export-ModuleMember `");
            writer.Write("-Alias");

            WriteStringArrayLiteral(
                writer, 
                aliases.Select(a => a.Item1)
            );

            writer.WriteLine("`");
            writer.Write("-Cmdlet");

            WriteStringArrayLiteral(
                writer,
                Commands.Select(c => c.Name).OrderBy(s => s)
            );
        }

        private void WriteStringArrayLiteral(
            TextWriter writer,
            IEnumerable<String> values
        )
        {
            writer.WriteLine(" @(");
            foreach (var s in values)
            {
                writer.WriteLine("    '{0}'", s);
            }
            writer.Write(") ");
        }
    }
}