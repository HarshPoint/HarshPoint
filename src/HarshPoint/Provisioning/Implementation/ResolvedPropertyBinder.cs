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
        public IReadOnlyCollection<PropertyAccessor> Properties { get; }

        public ResolvedPropertyBinder(IEnumerable<PropertyAccessor> properties)
        {
            if (properties == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(properties));
            }

            Properties = properties.ToImmutableArray();
        }

        /// <remarks>Mainly for unit tests.</remarks>
        public ResolvedPropertyBinder(Type type)
        {
            if (type == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(type));
            }

            Properties = new HarshObjectMetadata(type)
                .ModelProperties
                .Where(p => ResolvedPropertyTypeInfo.IsResolveType(p.PropertyTypeInfo))
                .ToImmutableArray();
        }

        public void Bind(Object target, Func<ResolveContext> resolveContextFactory)
        {
            if (target == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(target));
            }

            if (resolveContextFactory == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(resolveContextFactory));
            }

            var resolveCache = new ResolveCache();

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

            // save into array, otherwise we will create everything (especially ResolveContexts)
            // a-new when enumerating few lines down the road. and we certainly don't want to
            // have fresh uninitialized resolve contexts there.

            resolveBuilders = resolveBuilders.ToArray();

            foreach (var x in resolveBuilders)
            {
                Logger.Debug(
                    "Property {PropertyName} resolver {$Resolver} initializing context.",
                    x.Property.Name,
                    x.ResolveBuilder
                );

                x.ResolveContext.Cache = resolveCache;
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
                                    Failures = x.ResolveContext.Failures,
                                    x.Property,
                                    x.ResolveBuilder,
                                    ResultSource = resultSource,
                                };

            // save into array to force enumeration, so that all result sources 
            // are created before any properties are overwritten, because some 
            // of the resolve builders may depend on others, and they'd fail 
            // if they found the ResolveResult instead.

            resultSources = resultSources.ToArray();

            foreach (var x in resultSources)
            {
                var result = CreateResult(
                    x.Property,
                    x.ResolveBuilder,
                    x.ResultSource,
                    x.Failures
                );

                x.Property.SetValue(target, result);
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
            var value = property.GetValue(target);

            if (value == null)
            {
                Logger.Debug(
                    "Property {PropertyName} is null, skipping.",
                    property.Name
                );
            }

            return value;
        }

        private static Boolean ValidateResolveContext(ResolveContext resolveContext)
        {
            if (resolveContext == null)
            {
                throw Logger.Fatal.InvalidOperation(
                    SR.ResolveRegistrar_ContextFactoryReturnedNull
                );
            }

            return true;
        }

        private static IEnumerable CreateResultSource(
            String propertyName, 
            IResolveBuilder resolveBuilder, 
            ResolveContext resolveContext
        )
        {
            Logger.Debug(
                "Property {PropertyName} resolver {$Resolver} initializing.",
                propertyName,
                resolveBuilder
            );

            var resultSource = resolveBuilder.ToEnumerable(
                resolveContext,
                resolveBuilder.Initialize(resolveContext)
            );

            Logger.Debug(
                "Property {PropertyName} resolver {$Resolver} resolved into {$Value}.",
                propertyName,
                resolveBuilder,
                resultSource
            );

            return resultSource;
        }

        private static Object CreateResult(
            PropertyAccessor property,
            IResolveBuilder resolveBuilder,
            IEnumerable resultSource,
            IEnumerable<ResolveFailure> failureSource
        )
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
                resolveBuilder,
                failureSource
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
