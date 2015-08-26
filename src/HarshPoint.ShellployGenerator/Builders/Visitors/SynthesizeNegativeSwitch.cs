using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using SMA = System.Management.Automation;

namespace HarshPoint.ShellployGenerator.Builders
{
    internal sealed class SynthesizeNegativeSwitch : PropertyModelVisitor
    {
        private readonly HarshScopedValue<String> _renamed
            = new HarshScopedValue<String>();

        private readonly List<PropertyModel> _synthesized
            = new List<PropertyModel>();

        public override IEnumerable<PropertyModel> Visit(
            IEnumerable<PropertyModel> properties
        )
        {
            _synthesized.Clear();

            return base
                .Visit(properties)
                .Concat(_synthesized.ToImmutableArray());
        }

        protected internal override PropertyModel VisitNoNegative(
            PropertyModelNoNegative property
        )
        {
            if (property == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(property));
            }

            // do not visi but keep the rest of the chain
            return property.NextElement; 
        }

        protected internal override PropertyModel VisitRenamed(
            PropertyModelRenamed property
        )
        {
            if (property == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(property));
            }

            using (_renamed.EnterIfHasNoValue(property.PropertyName))
            {
                return base.VisitRenamed(property);
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

            if (property.PropertyType == typeof(Boolean?))
            {
                _synthesized.Add(
                    new PropertyModelSynthesized(
                        GetNegativeName(_renamed.Value ?? property.Identifier),
                        typeof(SMA.SwitchParameter),
                        property.Attributes
                    )
                );

                return NullableBoolToSwitch.Visit(property);
            }

            return base.VisitSynthesized(property);
        }

        private static String GetNegativeName(String name)
        {
            if (Regex.IsMatch(name, "^No[A-Z]"))
            {
                return name.Substring(2);
            }

            return $"No{name}";
        }

        private static readonly ChangePropertyTypeVisitor NullableBoolToSwitch
            = new ChangePropertyTypeVisitor(
                typeof(Boolean?), 
                typeof(SMA.SwitchParameter)
            );

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(SynthesizeNegativeSwitch));
    }
}
