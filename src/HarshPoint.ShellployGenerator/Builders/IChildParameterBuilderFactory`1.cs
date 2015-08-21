using System;

namespace HarshPoint.ShellployGenerator.Builders
{

    public interface IChildParameterBuilderFactory<TParent>:
        IChildParameterBuilderFactory
    {
        new IChildParameterBuilderFactory<TParent> Ignore();
        new IChildParameterBuilderFactory<TParent> SetFixedValue(Object value);
    }
}
