using System;

namespace HarshPoint.ShellployGenerator.Builders
{

    public interface IChildParameterBuilder<TParent>:
        IChildParameterBuilder
    {
        new IChildParameterBuilder<TParent> Ignore();
        new IChildParameterBuilder<TParent> SetFixedValue(Object value);
    }
}
