using System.IO;

namespace HarshPoint.ShellployGenerator.CodeGen
{
    public sealed class FileGeneratorContext
    {
        public FileGeneratorContext(DirectoryInfo outputDirectory)
        {
            if (outputDirectory == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(outputDirectory));
            }

            OutputDirectory = outputDirectory;
        }

        public DirectoryInfo OutputDirectory { get; }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(FileGeneratorContext));
    }
}
