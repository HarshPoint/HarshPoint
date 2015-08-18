using System;
using System.IO;
using System.Reflection;

namespace HarshPoint.ShellployGenerator
{
    internal static class Program
    {
        private static Int32 Main(String[] args)
        {
            if (!args.Any())
            {
                var assemblyFileName = Path.GetFileName(
                    Assembly.GetEntryAssembly().Location
                );

                Console.Error.WriteLine($"Usage: {assemblyFileName} outputDirectory");
                return 2;
            }

            try
            {
                var writer = new SourceFileWriter(args[0]);

                foreach (var command in ShellployMetadata.GetCommands())
                {
                    Console.WriteLine($"Generating {command.ClassName}...");
                    var targetUnit = new CommandCodeGenerator(command)
                        .GenerateCompileUnit();
                    writer.Write(targetUnit);
                }

                Console.WriteLine("Done.");
                return 0;
            }
            catch (Exception exc)
            {
                Console.Error.WriteLine(exc);
                return 1;
            }
        }
    }
}
