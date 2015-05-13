using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace HarshPoint.Reflection
{
    public static class MethodBaseExtensions
    {
        public static IEnumerable<T> Instance<T>(this IEnumerable<T> methods)
            where T : MethodBase
        {
            if (methods == null)
            {
                throw Error.ArgumentNull(nameof(methods));
            }

            return methods.Where(m => !m.IsStatic);
        }

        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "NonPublic")]
        public static IEnumerable<T> NonPublic<T>(this IEnumerable<T> methods)
            where T : MethodBase
        {
            if (methods == null)
            {
                throw Error.ArgumentNull(nameof(methods));
            }

            return methods.Where(m => !m.IsPublic);
        }

        public static IEnumerable<T> Static<T>(this IEnumerable<T> methods)
            where T : MethodBase
        {
            if (methods == null)
            {
                throw Error.ArgumentNull(nameof(methods));
            }

            return methods.Where(m => m.IsStatic);
        }
    }
}
