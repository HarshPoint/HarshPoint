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
            using (var writer = OpenFile(context))
            {
                Write(writer);
            }
        }

        protected TextWriter OpenFile(CodeGeneratorContext context)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

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

            return File.CreateText(path);
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(GeneratedFile));
    }
}
