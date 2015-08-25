using System;
using System.IO;

namespace HarshPoint.ShellployGenerator.CodeGen
{
    public abstract class FileGenerator
    {
        public String FileName { get; protected set; }

        protected abstract void Write(TextWriter writer);

        public virtual void Write(FileGeneratorContext context)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            var path = GetFilePath(context);

            Logger.Information("Generating {path}...", path);

            using (var writer = File.CreateText(path))
            {
                Write(writer);
            }
        }

        private String GetFilePath(FileGeneratorContext context)
        {
            if (FileName == null)
            {
                throw Logger.Fatal.InvalidOperation(
                    SR.GeneratedFile_FileNameNotSet
                );
            }

            var path = Path.Combine(
                context.OutputDirectory.FullName,
                FileName
            );

            return path;
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(FileGenerator));

    }
}
