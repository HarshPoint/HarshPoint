using HarshPoint.ShellployGenerator.Builders;
using HarshPoint.ShellployGenerator.CodeGen;
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
                Console.Error.WriteLine(exc);
                return 1;
            }
        }

        private static IEnumerable<GeneratedFile> CreateFiles(
            ShellployCommand[] commands
        )
        {
            var files = commands
                .Select(c => new GeneratedFileShellployCommand(c))
                .Cast<GeneratedFile>()
                .ToList();

            files.Add(new GeneratedFileAliases(commands));
            return files;
        }

        private static ShellployCommand[] CreateCommands()
        {
            var builderContext = new CommandBuilderContext();
            builderContext.AddBuildersFrom(typeof(Program).Assembly);

            return builderContext.Builders
                .Select(b => b.ToCommand(builderContext))
                .ToArray();
        }

        private static DirectoryInfo EnsureDirectoryEmpty(String path)
        {
            var directory = new DirectoryInfo(path);

            if (directory.Exists)
            {
                directory.Delete(recursive: true);
            }

            directory.Create();
            return directory;
        }
    }
}
