using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using SMA = System.Management.Automation;

namespace HarshPoint.ShellployGenerator
{
    internal class ShellployCommandPropertyParameterAttribute
    {
        public Boolean Mandatory { get; internal set; }
        public String ParameterSet { get; internal set; }
        public Int32? Position { get; internal set; }

        public Tuple<String, Object>[] GetAttributeArguments()
        {
            var result = new List<Tuple<String, Object>>()
            {
                Tuple.Create<String, Object>(nameof(SMA.ParameterAttribute.Mandatory),  Mandatory),
                Tuple.Create<String, Object>(nameof(SMA.ParameterAttribute.ParameterSetName),ParameterSet),

                Tuple.Create<String, Object>(nameof(SMA.ParameterAttribute.ValueFromPipelineByPropertyName), true),
            };

            if (Position != null)
            {
                result.Add(Tuple.Create<String, Object>(nameof(SMA.ParameterAttribute.Position), Position));
            }

            return result.ToArray();
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<ShellployCommandPropertyParameterAttribute>();
    }
}