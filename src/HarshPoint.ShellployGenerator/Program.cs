using HarshPoint.ShellployGenerator.Builders;
using HarshPoint.ShellployGenerator.CodeGen;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.FormattableString;

namespace HarshPoint.ShellployGenerator
{
    internal static class Program
    {
        private static String ProgramName =>
            Environment.GetCommandLineArgs().FirstOrDefault();

        private static Int32 Main(String[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine(
                    Invariant($"Usage: {ProgramName} OutputDirectory")
                );
                return 2;
            }

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                var builderContext = new CommandBuilderContext();
                builderContext.AddBuildersFrom(typeof(Program).Assembly);

                var generators = builderContext.Builders
                    .Select(b => b.ToCodeGenerator())
                    .ToArray();

                var directory = EnsureDirectoryEmpty(args[0]);
                var context = new FileGeneratorContext(directory);

                foreach (var file in generators)
                {
                    file.Write(context);
                }

                var aliases = new AliasFileGenerator(
                    generators.Select(g => g.Command)
                );

                aliases.Write(context);

                return 0;
            }
            catch (Exception exc)
            {
                Log.Fatal(exc, "Unhandled exception");
                throw;
            }
        }

        private static DirectoryInfo EnsureDirectoryEmpty(String path)
        {
            var directory = new DirectoryInfo(path);

            if (directory.Exists)
            {
                Log.Warning("Recursively deleting {path}", path);
                directory.Delete(recursive: true);
            }

            Log.Information("Creating directory {path}", path);
            directory.Create();
            return directory;
        }
    }
}
