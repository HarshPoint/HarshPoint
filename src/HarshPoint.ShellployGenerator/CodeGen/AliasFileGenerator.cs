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
            Aliases = ImmutableArray.CreateRange(
                from cmd in commands
                from alias in cmd.Aliases.DefaultIfEmpty()
                orderby alias
                select Tuple.Create(alias, cmd.Name)
            );

            Aliases = Aliases.Add(
                Tuple.Create("Provision", "Invoke-Provisioner")
            );

            Aliases = Aliases.Add(
                Tuple.Create((String)null, "Invoke-WithProvisionerContext")
            );

            FileName = "HarshPoint.Shellploy.psm1";
        }

        public ImmutableArray<Tuple<String, String>> Aliases { get; }

        protected override void Write(TextWriter writer)
        {
            foreach (var a in Aliases.Where(a => a.Item1 != null))
            {
                writer.WriteLine(
                    $"Set-Alias -Name {a.Item1} -Value {a.Item2}"
                );
            }

            writer.WriteLine();
            writer.WriteLine("Export-ModuleMember `");
            writer.Write("-Alias");

            WriteStringArrayLiteral(
                writer,
                Aliases
                    .Select(a => a.Item1)
                    .Where(s => s != null)
            );

            writer.WriteLine("`");
            writer.Write("-Cmdlet");

            WriteStringArrayLiteral(
                writer,
                Aliases
                    .Select(a => a.Item2)
                    .Distinct()
                    .OrderBy(s => s)
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