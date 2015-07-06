using HarshPoint.Provisioning.Implementation;
using System;
using System.Reflection;

namespace HarshPoint.Provisioning
{
    public abstract class ParameterValidationAttribute : Attribute
    {
        private readonly HarshScopedValue<Parameter> _parameter =
            new HarshScopedValue<Parameter>();

        private HarshLogger _logger;

        protected HarshLogger Logger
            => HarshLazy.Initialize(
                ref _logger, 
                () => HarshLog.ForContext(GetType())
            );

        protected String ParameterName
            => Parameter?.Name;

        protected Type ParameterType
            => Parameter?.PropertyType;

        protected TypeInfo ParameterTypeInfo
           => Parameter?.PropertyTypeInfo;

        internal Parameter Parameter
            => _parameter.Value;

        internal void Validate(Parameter parameter, Object value)
        {
            if (parameter == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(parameter));
            }

            using (_parameter.Enter(parameter))
            {
                Validate(value);
            }
        }

        protected abstract void Validate(Object value);

        protected Exception ValidationFailed(String format, params Object[] args)
        {
            return Logger.Error.ParameterValidationFormat(
                Parameter, format, args
            );
        }
    }
}
