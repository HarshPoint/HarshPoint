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

        public void Bind<TContext>(Object target, TContext context)
            where TContext : HarshProvisionerContextBase
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

                var resolveBuilder = value as IResolveBuilder<TContext>;

                if (resolveBuilder == null)
                {
                    Logger.Debug(
                        "Parameter {$Parameter} value {$Value} is not an IResolveBuilder, skipping.",
                        parameter,
                        value
                    );

                    continue;
                }

                var resolveContext = new ResolveContext<TContext>(context);

                Logger.Debug(
                    "Parameter {$Parameter} resolver {$Resolver} initializing.",
                    parameter,
                    resolveBuilder
                );

                var resultSource = resolveBuilder.ToEnumerable(
                    resolveBuilder.Initialize(resolveContext),
                    resolveContext
                );

                if (resultSource == null)
                {
                    Logger.Warning(
                        "Parameter {$Parameter} resolver {$Resolver} resolved into null value.",
                        parameter,
                        resolveBuilder
                    );

                    parameter.Setter(target, null);
                    continue;
                }

                Logger.Debug(
                    "Parameter {$Parameter} resolver {$Resolver} resolved into value {$Value}.",
                    parameter,
                    resolveBuilder,
                    resultSource
                );

                var result = ResolveResultFactory.CreateResult(
                    parameter.PropertyTypeInfo, 
                    resultSource,
                    resolveBuilder
                );

                Logger.Debug(
                    "Parameter {$Parameter} resolver {$Resolver} result adapted into {$Value}, assigning.",
                    parameter,
                    resolveBuilder,
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
    }
}
