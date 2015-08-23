using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace HarshPoint.ShellployGenerator.Builders
{
    public static class VisitorExtensions
    {
        public static IEnumerable<T> Visit<T>(
            this IVisitor<T> visitor,
            IEnumerable<T> values
        )
        {
            if (visitor == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(visitor));
            }

            if (values == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(values));
            }

            return values
                .Select(visitor.Visit)
                .Where(result => result != null)
                .ToImmutableArray();
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(VisitorExtensions));
    }
}
