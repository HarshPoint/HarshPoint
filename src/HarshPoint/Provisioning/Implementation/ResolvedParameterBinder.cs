using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ResolvedParameterBinder
    {
        private static readonly HarshLogger Logger = HarshLog.ForContext<ResolvedParameterBinder>();

        public ResolvedParameterBinder(IEnumerable<Parameter> parameters)
        {
            if (parameters == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(parameters));
            }

            Parameters = parameters
                .Where(p => p.ResolvedType != null)
                .ToImmutableArray();
        }

        public void Bind(Object target, IResolveContext context)
        {
            if (target == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(target));
            }

            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            foreach (var parameter in Parameters)
            {
                var value = parameter.Getter(target);

                if (value == null)
                {
                    Logger.Debug(
                        "Parameter {$Parameter} is null, skipping.",
                        parameter
                    );

                    continue;
                }

                var resolver = value as IIndirectResolver;

                if (resolver == null)
                {
                    Logger.Debug(
                        "Parameter {$Parameter} value {$Value} is not an IResolver, skipping.",
                        parameter,
                        value
                    );

                    continue;
                }

                var result = resolver.Initialize(context);

                if (result == null)
                {
                    Logger.Warning(
                        "Parameter {$Parameter} resolver {$Resolver} resolved into null value.",
                        parameter,
                        resolver
                    );

                    parameter.Setter(target, null);

                    continue;
                }

                if (!parameter.PropertyTypeInfo.IsAssignableFrom(result.GetType().GetTypeInfo()))
                {
                    throw ResolveResultNotCompatible(
                        parameter,
                        resolver,
                        result
                    );
                }

                Logger.Debug(
                    "Parameter {$Parameter} resolver {$Resolver} resolved into value {$Value}, assigning.",
                    parameter,
                    resolver,
                    result
                );

                parameter.Setter(target, result);
            }
        }

        public IReadOnlyCollection<Parameter> Parameters
        {
            get;
            private set;
        }

        private static InvalidOperationException ResolveResultNotCompatible(Parameter parameter, IIndirectResolver resolver, Object result)
        {
            if (parameter == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(parameter));
            }

            if (resolver == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(resolver));
            }

            if (result == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(result));
            }

            return Logger.Fatal.InvalidOperationFormat(
                SR.ResolvedParameterBinder_ResolverResultNotCompatible,
                parameter.Name,
                resolver,
                result,
                parameter.PropertyType
            );
        }
    }
}
