using System;

namespace HarshPoint.ShellployGenerator.Builders
{
    public interface IChildParameterBuilderFactory
    {
        IChildParameterBuilderFactory Ignore();
        IChildParameterBuilderFactory SetFixedValue(Object value);
    }
}
