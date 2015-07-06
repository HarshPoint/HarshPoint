using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class DefaultFromContextParameterBinder
    {
        private static readonly HarshLogger Logger = HarshLog.ForContext<DefaultFromContextParameterBinder>();

        public DefaultFromContextParameterBinder(IEnumerable<Parameter> parameters)
        {
            if (parameters == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(parameters));
            }

            DefaultFromContextParameters = parameters
                .Where(p => p.IsDefaultFromContext)
                .ToImmutableArray();
        }

        public void Bind(Object target, HarshProvisionerContextBase context)
        {
            if (target == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(target));
            }

            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            foreach (var param in DefaultFromContextParameters)
            {
                if (!param.HasDefaultValue(target))
                {
                    Logger.Debug(
                        "Parameter {Parameter} already has non-default value, skipping",
                        param
                    );

                    continue;
                }

                var value = GetValueFromContext(param, context);

                if (value != null)
                {
                    Logger.Debug(
                        "Setting parameter {Parameter} to {Value}",
                        param,
                        value
                    );

                    param.Setter(target, value);
                }
                else
                {
                    Logger.Debug(
                        "Got null from context for parameter {Parameter}, skipping",
                        param
                    );
                }
            }
        }

        public IImmutableList<Parameter> DefaultFromContextParameters
        {
            get;
            private set;
        }

        private static Object GetValueFromContext(Parameter param, HarshProvisionerContextBase context)
        {
            if (param.DefaultFromContextTagType != null)
            {
                Logger.Debug(
                    "Parameter {Parameter} gets default value form context by the tag type {TagType}",
                    param,
                    param.DefaultFromContextTagType
                );

                return context
                    .GetState(param.DefaultFromContextTagType)
                    .Cast<IDefaultFromContextTag>()
                    .FirstOrDefault()?
                    .Value;
            }

            if (param.ResolvedType != null)
            {
                Logger.Debug(
                    "Parameter {Parameter} resolves objects of type {ResolvedType}",
                    param,
                    param.ResolvedType
                );

                return ContextStateResolver.Create(
                    param.ResolvedType
                );
            }

            Logger.Debug(
                "Parameter {Parameter} gets default value from context directly by its own type {ParameterType}",
                param,
                param.PropertyType
            );

            return context
                .GetState(param.PropertyType)
                .FirstOrDefault();
        }
    }
}
