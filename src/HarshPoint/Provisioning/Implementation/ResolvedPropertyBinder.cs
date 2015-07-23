using HarshPoint.ObjectModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

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

            foreach (var property in Properties)
            {
                ResolveProperty(target, property, resolveContextFactory);
            }
        }

        private void ResolveProperty(Object target, PropertyAccessor property, Func<IResolveContext> resolveContextFactory)
        {
            var value = GetValue(target, property);

            if (value == null)
            {
                return;
            }

            var resolveBuilder = value as IResolveBuilder;

            if (resolveBuilder == null)
            {
                Logger.Debug(
                    "Property {PropertyName} value {$Value} is not an IResolveBuilder, skipping.",
                    property.Name,
                    value
                );

                return;
            }

            var resolveContext = resolveContextFactory();

            if (resolveContext == null)
            {
                throw Logger.Fatal.InvalidOperation(
                    SR.ResolveRegistrar_ContextFactoryReturnedNull
                );
            }

            Logger.Debug(
                "Property {PropertyName} resolver {$Resolver} initializing context.",
                property.Name,
                resolveBuilder
            );

            resolveBuilder.InitializeContext(resolveContext);

            var resultSource = CreateResultSource(
                property,
                resolveBuilder,
                resolveContext
            );

            if (resultSource == null)
            {
                Logger.Warning(
                    "Property {PropertyName} resolver {$Resolver} resolved into null value.",
                    property.Name,
                    resolveBuilder
                );

                property.Setter(target, null);
                return;
            }

            var result = CreateResult(
                property,
                resultSource,
                resolveBuilder
            );

            property.Setter(target, result);
        }

        private Object GetValue(Object target, PropertyAccessor property)
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

        private IEnumerable CreateResultSource(PropertyAccessor property, IResolveBuilder resolveBuilder, IResolveContext resolveContext)
        {
            Logger.Debug(
                "Property {PropertyName} resolver {$Resolver} initializing.",
                property.Name,
                resolveBuilder
            );

            var resultSource = resolveBuilder.ToEnumerable(
                resolveBuilder.Initialize(resolveContext),
                resolveContext
            );

            Logger.Debug(
                "Property {PropertyName} resolver {$Resolver} resolved into {$Value}.",
                property.Name,
                resolveBuilder,
                resultSource
            );

            return resultSource;
        }

        private Object CreateResult(PropertyAccessor property, IEnumerable resultSource, IResolveBuilder resolveBuilder)
        {
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
