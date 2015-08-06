using System;

namespace HarshPoint.ObjectModel
{
    public abstract class ParameterValidationAttribute : Attribute
    {
        protected internal abstract void Validate(Parameter parameter, Object value);
    }
}
