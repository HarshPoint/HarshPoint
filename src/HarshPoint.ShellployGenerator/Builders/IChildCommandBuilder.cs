using System;
using System.Collections.Generic;

namespace HarshPoint.ShellployGenerator.Builders
{
    internal interface IChildCommandBuilder
    {
        IEnumerable<ShellployCommandProperty> Process(
            IEnumerable<ShellployCommandProperty> parentProperties
        );
        Type ProvisionerType { get; }

//        IChildParameterBuilder Parameter(String name)
    }

    internal interface IChildParameterBuilder
    {
        void Ignore();
    }
}