using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SMA = System.Management.Automation;

namespace HarshPoint.ShellployGenerator.Builders
{
    internal sealed class IgnoreUnfixedParameterSetPropertiesVisitor :
        PropertyModelVisitor
    {
        private ImmutableHashSet<String> _unfixedSets;

        public override IEnumerable<PropertyModel> Visit(
            IEnumerable<PropertyModel> properties
        )
        {
            if (properties == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(properties));
            }

            var allSets = new FindAllParameterSets();
            allSets.Visit(properties);

            if (allSets.ParameterSets.IsEmpty || !allSets.HasFixedProperties)
            {
                return properties;
            }

            var unfixedSets = new FindUnfixedParameterSets(
                allSets.ParameterSets
            );

            unfixedSets.Visit(properties);
            _unfixedSets = unfixedSets.ParameterSets;

            return base.Visit(properties);
        }

        protected internal override PropertyModel VisitSynthesized(
            PropertyModelSynthesized property
        )
        {
            if (property == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(property));
            }

            var propertySets = GetPropertyParameterSets(property);

            if (!propertySets.Any())
            {
                // property belongs to all parameter sets
                return base.VisitSynthesized(property);
            }

            var fixedSets = propertySets
                .Except(_unfixedSets, _unfixedSets.KeyComparer);

            if (!fixedSets.Any())
            {
                return new PropertyModelIgnored(property);
            }

            return base.VisitSynthesized(property);
        }
        
        private static IEnumerable<String> GetPropertyParameterSets(
            PropertyModelSynthesized property
        )
            => from a in property.Attributes
               where a.AttributeType == typeof(SMA.ParameterAttribute)

               from kvp in a.Properties
               where kvp.Key == "ParameterSetName"

               select (String)kvp.Value;

        private sealed class FindAllParameterSets : PropertyModelVisitor
        {
            public ImmutableHashSet<String> ParameterSets { get; private set; }
                = ImmutableHashSet.Create<String>(StringComparer.OrdinalIgnoreCase);

            public Boolean HasFixedProperties { get; private set; }

            protected internal override PropertyModel VisitFixed(PropertyModelFixed property)
            {
                HasFixedProperties = true;
                return base.VisitFixed(property);
            }

            protected internal override PropertyModel VisitSynthesized(
                PropertyModelSynthesized property
            )
            {
                if (property == null)
                {
                    throw Logger.Fatal.ArgumentNull(nameof(property));
                }

                ParameterSets = ParameterSets.Union(
                    GetPropertyParameterSets(property)
                );

                return base.VisitSynthesized(property);
            }
        }

        private sealed class FindUnfixedParameterSets : PropertyModelVisitor
        {
            private readonly HarshScopedValue<Boolean> _isFixed
                = new HarshScopedValue<Boolean>();

            public FindUnfixedParameterSets(
                IEnumerable<String> allParameterSetNames
            )
            {
                ParameterSets = ImmutableHashSet.CreateRange(
                    StringComparer.OrdinalIgnoreCase,
                    allParameterSetNames
                );
            }

            public ImmutableList<String> FixedProperties { get; private set; }
                = ImmutableList<String>.Empty;

            public ImmutableHashSet<String> ParameterSets { get; private set; }

            protected internal override PropertyModel VisitFixed(
                PropertyModelFixed property
            )
            {
                if (property == null)
                {
                    throw Logger.Fatal.ArgumentNull(nameof(property));
                }

                using (_isFixed.Enter(true))
                {
                    return base.VisitFixed(property);
                }
            }

            protected internal override PropertyModel VisitSynthesized(
                PropertyModelSynthesized property
            )
            {
                if (property == null)
                {
                    throw Logger.Fatal.ArgumentNull(nameof(property));
                }

                if (_isFixed.Value)
                {
                    FixedProperties = FixedProperties.Add(property.Identifier);

                    ParameterSets = ParameterSets.Except(
                        GetPropertyParameterSets(property)
                    );

                    if (ParameterSets.IsEmpty)
                    {
                        throw Logger.Fatal.InvalidOperationFormat(
                            SR.IgnoreUnfixedParameterSetPropertiesVisitor_ExclusivePropertiesAreFixed,
                            String.Join(", ", FixedProperties)
                        );
                    }

                }

                return base.VisitSynthesized(property);
            }
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(IgnoreUnfixedParameterSetPropertiesVisitor));
    }
}
