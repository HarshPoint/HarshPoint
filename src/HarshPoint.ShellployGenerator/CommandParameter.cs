using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace HarshPoint.ShellployGenerator
{
    internal abstract class CommandParameter
    {
        internal CommandParameter Previous { get; set; }

        internal Int32? SortOrder { get; set; }

        internal virtual IEnumerable<ShellployCommandProperty> Synthesize()
        {
            if (Previous == null)
            {
                return ImmutableArray<ShellployCommandProperty>.Empty;
            }

            var properties = Previous.Synthesize();

            foreach (var prop in properties)
            {
                Process(prop);
            }

            return properties;
        }

        internal virtual void Process(ShellployCommandProperty property)
        {
        }

        internal virtual CommandParameter CreateFrom(CommandParameter previous)
        {
            if (previous != null)
            {
                Previous = previous;
                SortOrder = SortOrder ?? previous.SortOrder;
            }

            return this;
        }
    }
}
