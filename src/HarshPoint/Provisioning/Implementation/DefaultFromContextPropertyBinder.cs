using HarshPoint.ObjectModel;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class DefaultFromContextPropertyBinder
    {
        private static readonly HarshLogger Logger = HarshLog.ForContext<DefaultFromContextPropertyBinder>();

        public DefaultFromContextPropertyBinder(IEnumerable<DefaultFromContextProperty> properties)
        {
            if (properties == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(properties));
            }

            Properties = properties.ToImmutableArray();
        }

        public void Bind(ITrackValueSource target, IHarshProvisionerContext context)
        {
            if (target == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(target));
            }

            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            foreach (var prop in Properties)
            {
                var value = prop.Accessor.GetValue(target);

                if (value != null)
                {
                    Logger.Debug(
                        "Property {PropertyName} already has non-null value, skipping",
                        prop.Name
                    );

                    continue;
                }

                value = GetValueFromContext(prop, context);

                if (value != null)
                {
                    Logger.Debug(
                        "Setting property {PropertyName} to {$Value}",
                        prop.Name,
                        value
                    );

                    prop.Accessor.SetValue(target, value, DefaultFromContextPropertyValueSource.Instance);
                }
                else
                {
                    Logger.Debug(
                        "Got null from context for property {PropertyName}, skipping",
                        prop.Name
                    );
                }
            }
        }

        public IImmutableList<DefaultFromContextProperty> Properties
        {
            get;

        }

        private static Object GetValueFromContext(DefaultFromContextProperty prop, IHarshProvisionerContext context)
        {
            if (prop.TagType != null)
            {
                Logger.Debug(
                    "Property {PropertyName} gets default value form context by the tag type {TagType}",
                    prop.Name,
                    prop.TagType
                );

                return context
                    .GetState(prop.TagType)
                    .Cast<IDefaultFromContextTag>()
                    .FirstOrDefault()?
                    .Value;
            }

            if (prop.ResolvedPropertyInfo != null)
            {
                Logger.Debug(
                    "Property {PropertyName} resolves objects of type {ResolvedType}",
                    prop.Name,
                    prop.ResolvedPropertyInfo.ResolvedType
                );

                return ContextStateResolveBuilder.Create(
                    prop.ResolvedPropertyInfo.ResolvedType
                );
            }

            Logger.Debug(
                "Property {PropertyName} gets default value from context directly by its own type {PropertyType}",
                prop.Name,
                prop.PropertyType
            );

            return context
                .GetState(prop.PropertyType)
                .FirstOrDefault();
        }
    }
}
