using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CSharp;

namespace HarshPoint.ShellployGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var provider = new CSharpCodeProvider();
            var generator = new CommandCodeGenerator();

            foreach (var command in new ShellployMetadata().GetCommands())
            {
                var targetUnit = generator.GenerateCompileUnit(command);

                using (var sourceWriter = new StreamWriter($"{ command.ClassName}.cs"))
                {
                    provider.GenerateCodeFromCompileUnit(targetUnit, sourceWriter, new CodeGeneratorOptions()
                    {
                        BracingStyle = "C",
                    });
                }
            }
        }
    }
}

