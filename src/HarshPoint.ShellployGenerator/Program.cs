﻿using System;
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
    internal class Program
    {
        static void Main(String[] args)
        {
            if (!args.Any())
            {
                var assemblyFileName = Path.GetFileName(
                    Assembly.GetEntryAssembly().Location
                );

                Console.WriteLine($"Usage: {assemblyFileName} outputDirectory");
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