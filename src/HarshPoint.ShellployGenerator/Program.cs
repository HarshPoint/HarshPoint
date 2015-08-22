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
                var commands = CreateCommands();
                var files = CreateFiles(commands);

                var directory = EnsureDirectoryEmpty(args[0]);
                var codeGenContext = new CodeGeneratorContext(directory);

                foreach (var file in files)
                {
                    file.Write(codeGenContext);
                }

                return 0;
            }
            catch (Exception exc)
            {
                Log.Fatal(exc, "Unhandled exception");
                throw;
            }
        }

        private static IEnumerable<GeneratedFile> CreateFiles(
            IEnumerable<CommandBuilder> commands
        )
        {
            var files = commands
                .Select(c => new GeneratedFileCommand(c))
                .Cast<GeneratedFile>()
                .ToList();

            files.Add(new GeneratedFileAliases(commands));
            return files;
        }

        private static IEnumerable<CommandBuilder> CreateCommands()
        {
            var builderContext = new CommandBuilderContext();
            builderContext.AddBuildersFrom(typeof(Program).Assembly);

            return builderContext.Builders;
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
