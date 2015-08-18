using System;
using System.Collections.Generic;
using SMA = System.Management.Automation;

namespace HarshPoint.ShellployGenerator
{
    internal class ShellployCommandPropertyParameterAttribute
    {
        public Boolean Mandatory { get; set; }
        public String ParameterSet { get; set; }
        public Int32? Position { get; set; }
        public Boolean ValueFromPipeline { get; set; }

        public Tuple<String, Object>[] GetAttributeArguments()
        {
            var result = new List<Tuple<String, Object>>()
            {
                Tuple.Create<String, Object>(nameof(SMA.ParameterAttribute.ValueFromPipelineByPropertyName), true),
            };

            if (ValueFromPipeline)
            {
                result.Add(Tuple.Create<String, Object>(nameof(SMA.ParameterAttribute.ValueFromPipeline), ValueFromPipeline));
            }

            if (Mandatory)
            {
                result.Add(Tuple.Create<String, Object>(nameof(SMA.ParameterAttribute.Mandatory), Mandatory));
            }

            if (ParameterSet != null)
            {
                result.Add(Tuple.Create<String, Object>(nameof(SMA.ParameterAttribute.ParameterSetName), ParameterSet));
            }

            if (Position != null)
            {
                result.Add(Tuple.Create<String, Object>(nameof(SMA.ParameterAttribute.Position), Position));
            }

            return result.ToArray();
        }
    }
}