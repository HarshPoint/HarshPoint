using Microsoft.CSharp;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;

namespace HarshPoint.ShellployGenerator.CodeGen
{
    public abstract class CodeDomGenerator : FileGenerator
    {
        protected override void Write(TextWriter writer)
        {
            if (writer == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(writer));
            }

            var unit = ToCodeCompileUnit();
            
            if (FileName == null)
            {
                var type = unit.Namespaces
                    .Cast<CodeNamespace>()
                    .SelectMany(ns => ns.Types.Cast<CodeTypeDeclaration>())
                    .FirstOrDefault();

                if (type == null)
                {
                    throw Logger.Fatal.InvalidOperation(
                        SR.SourceFileWriter_NoTypeDefined
                    );
                }

                FileName = $"{type.Name}.cs";
            }

            using (var provider = new CSharpCodeProvider())
            {
                provider.GenerateCodeFromCompileUnit(
                    unit,
                    writer,
                    CodeGeneratorOptions
                );
            }
        }

        protected abstract CodeCompileUnit ToCodeCompileUnit();

        private static readonly CodeGeneratorOptions CodeGeneratorOptions
            = new CodeGeneratorOptions()
            {
                BlankLinesBetweenMembers = true,
                BracingStyle = "C",
            };

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(CodeDomGenerator));
    }
}
