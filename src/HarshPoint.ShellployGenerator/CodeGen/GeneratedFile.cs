using System;
using System.IO;

namespace HarshPoint.ShellployGenerator.CodeGen
{
    internal abstract class GeneratedFile
    {
        protected String FileName { get; set; }

        protected abstract void Write(TextWriter writer);

        public virtual void Write(CodeGeneratorContext context)
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

        private string GetFilePath(CodeGeneratorContext context)
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
            = HarshLog.ForContext(typeof(GeneratedFile));

    }
}
