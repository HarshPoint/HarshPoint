using System.Collections.Generic;
using System.IO;

namespace HarshPoint.ShellployGenerator.CodeGen
{
    internal sealed class CodeGeneratorContext
    {
        public CodeGeneratorContext(DirectoryInfo outputDirectory)
        {
            if (outputDirectory == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(outputDirectory));
            }

            OutputDirectory = outputDirectory;
        }

        public DirectoryInfo OutputDirectory { get; }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(CodeGeneratorContext));
    }
}
