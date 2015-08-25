using HarshPoint.ShellployGenerator.Builders;
using HarshPoint.ShellployGenerator.CodeGen;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
                Console.Error.WriteLine($"Usage: {ProgramName} OutputDirectory");
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
                    RunGenerator(file, context);
                }

                var aliases = new AliasFileGenerator(
                    generators.Select(g => g.Command)
                );

                RunGenerator(aliases, context);

                return 0;
            }
            catch (Exception exc)
            {
                Log.Fatal(exc, "Unhandled exception");
                throw;
            }
        }

        private static void RunGenerator(
            FileGenerator generator, 
            FileGeneratorContext context
        )
        {
            try
            {
                generator.Write(context);
            }
            catch (Exception exc)
            {
                throw new Exception(
                    $"Error generating {generator.FileName}",
                    exc
                );
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
