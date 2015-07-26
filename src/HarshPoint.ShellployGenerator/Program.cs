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
using System.Diagnostics;

namespace HarshPoint.ShellployGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!args.Any())
            {
                Console.Write($"Usage: {Process.GetCurrentProcess().ProcessName} outputDirectory");
                return;
            }

            var generator = new CommandCodeGenerator();

            var writer = new SourceFileWriter(args[0]);

            foreach (var command in new ShellployMetadata().GetCommands())
            {
                var targetUnit = generator.GenerateCompileUnit(command);
                writer.Write(targetUnit);
            }
        }
    }
}
