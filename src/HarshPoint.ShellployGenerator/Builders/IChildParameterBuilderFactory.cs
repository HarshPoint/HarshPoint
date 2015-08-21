using System;
using System.Collections.Generic;

namespace HarshPoint.ShellployGenerator.Builders
{
    public interface IChildParameterBuilderFactory<TParent>
    {
        IChildParameterBuilderFactory<TParent> Ignore();
        IChildParameterBuilderFactory<TParent> SetFixedValue(Object value);
    }
}
