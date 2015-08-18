using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;

namespace HarshPoint.ShellployGenerator
{
    internal class SourceFileWriter
    {
        private static CodeDomProvider CodeProvider = new CSharpCodeProvider();

        private static CodeGeneratorOptions CodeGeneratorOptions = new CodeGeneratorOptions()
        {
            BracingStyle = "C",
        };

        private String _outputDirectory;

        public SourceFileWriter(String outputDirectory)
        {
            if (String.IsNullOrEmpty(outputDirectory))
            {
                throw Logger.Fatal.ArgumentNullOrEmpty(nameof(outputDirectory));
            }

            if (File.Exists(outputDirectory))
            {
                throw Logger.Fatal.Argument(nameof(outputDirectory), SR.SourceFileWriter_OutputDirectoryIsFile);
            }

            if (Directory.Exists(outputDirectory))
            {
                Directory.Delete(outputDirectory, recursive: true);
            }

            Directory.CreateDirectory(outputDirectory);

            _outputDirectory = outputDirectory;
        }

        public void Write(CodeCompileUnit compileUnit)
        {
            var filePath = Path.Combine(
                _outputDirectory,
                GetFileName(compileUnit)
            );

            using (var sourceWriter = new StreamWriter(filePath))
            {
                CodeProvider.GenerateCodeFromCompileUnit(compileUnit, sourceWriter, CodeGeneratorOptions);
            }
        }

        private static String GetFileName(CodeCompileUnit compileUnit)
        {
            var types = compileUnit.Namespaces
                .Cast<CodeNamespace>()
                .SelectMany(ns => ns.Types.Cast<CodeTypeDeclaration>());

            if (!types.Any())
            {
                throw Logger.Fatal.Argument(nameof(compileUnit), SR.SourceFileWriter_NoTypeDefined);
            }

            return $"{types.First().Name}.cs";
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<SourceFileWriter>();
    }
}