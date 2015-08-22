using System;

namespace HarshPoint.ShellployGenerator.Builders
{
    public interface IChildParameterBuilder
    {
        IChildParameterBuilder Ignore();
        IChildParameterBuilder SetFixedValue(Object value);
    }
}
