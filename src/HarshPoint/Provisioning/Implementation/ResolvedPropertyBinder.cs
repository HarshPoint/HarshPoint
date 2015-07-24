using HarshPoint.ObjectModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ResolvedPropertyBinder
    {
        public IReadOnlyCollection<PropertyAccessor> Properties { get; private set; }

        public ResolvedPropertyBinder(IEnumerable<PropertyAccessor> properties)
        {
            if (properties == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(properties));
            }

            Properties = properties.ToImmutableArray();
        }

        public void Bind(Object target, Func<IResolveContext> resolveContextFactory)
        {
            if (target == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(target));
            }

            if (resolveContextFactory == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(resolveContextFactory));
            }

            var resolveBuilders = from property in Properties

                                  let value = GetValue(property, target)
                                  where value != null

                                  let resolveBuilder = GetResolveBuilder(property, value)
                                  where resolveBuilder != null

                                  let resolveContext = resolveContextFactory()
                                  where ValidateResolveContext(resolveContext)

                                  select new
                                  {
                                      Property = property,
                                      ResolveBuilder = resolveBuilder,
                                      ResolveContext = resolveContext,
                                  };

            foreach (var x in resolveBuilders)
            {
                Logger.Debug(
                    "Property {PropertyName} resolver {$Resolver} initializing context.",
                    x.Property.Name,
                    x.ResolveBuilder
                );

                x.ResolveBuilder.InitializeContext(x.ResolveContext);
            }

            var resultSources = from x in resolveBuilders
                                let resultSource = CreateResultSource(
                                    x.Property.Name,
                                    x.ResolveBuilder,
                                    x.ResolveContext
                                )
                                select new
                                {
                                    x.Property,
                                    x.ResolveBuilder,
                                    ResultSource = resultSource
                                };

            // force enumeration, so that all result sources are created
            // before any properties are overwritten, because some of the 
            // resolve builders may depend on others, and would fail if they
            // found the ResolveResult instead.

            resultSources = resultSources.ToArray();
            
            foreach (var x in resultSources)
            {
                var result = CreateResult(
                    x.Property,
                    x.ResolveBuilder,
                    x.ResultSource
                );

                x.Property.Setter(target, result);
            }
        }

        private static IResolveBuilder GetResolveBuilder(PropertyAccessor property, Object value)
        {
            var resolveBuilder = value as IResolveBuilder;

            if (resolveBuilder == null)
            {
                Logger.Debug(
                    "Property {PropertyName} value {$Value} is not an IResolveBuilder, skipping.",
                    property.Name,
                    value
                );
            }

            return resolveBuilder;
        }

        private static Object GetValue(PropertyAccessor property, Object target)
        {
            var value = property.Getter(target);

            if (value == null)
            {
                Logger.Debug(
                    "Property {PropertyName} is null, skipping.",
                    property.Name
                );
            }

            return value;
        }

        private static Boolean ValidateResolveContext(IResolveContext resolveContext)
        {
            if (resolveContext == null)
            {
                throw Logger.Fatal.InvalidOperation(
                    SR.ResolveRegistrar_ContextFactoryReturnedNull
                );
            }

            return true;
        }

        private static IEnumerable CreateResultSource(String propertyName, IResolveBuilder resolveBuilder, IResolveContext resolveContext)
        {
            Logger.Debug(
                "Property {PropertyName} resolver {$Resolver} initializing.",
                propertyName,
                resolveBuilder
            );

            var resultSource = resolveBuilder.ToEnumerable(
                resolveBuilder.Initialize(resolveContext),
                resolveContext
            );

            Logger.Debug(
                "Property {PropertyName} resolver {$Resolver} resolved into {$Value}.",
                propertyName,
                resolveBuilder,
                resultSource
            );

            return resultSource;
        }

        private static Object CreateResult(PropertyAccessor property, IResolveBuilder resolveBuilder, IEnumerable resultSource)
        {
            if (resultSource == null)
            {
                Logger.Warning(
                    "Property {PropertyName} resolver {$Resolver} resolved into null value.",
                    property.Name,
                    resolveBuilder
                );

                return null;
            }

            var result = ResolveResultFactory.CreateResult(
                property.PropertyTypeInfo,
                resultSource,
                resolveBuilder
            );

            Logger.Debug(
                "Property {PropertyName} resolver {$Resolver} result adapted into {$Value}, assigning.",
                property.Name,
                resolveBuilder,
                result
            );

            return result;
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<ResolvedPropertyBinder>();
    }
}
