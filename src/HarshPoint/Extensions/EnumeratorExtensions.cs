using System.Collections.Generic;

namespace HarshPoint
{
    public static class EnumeratorExtensions
    {
        public static T GetNextOrFail<T>(this IEnumerator<T> enumerator)
        {
            if (enumerator == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(enumerator));
            }

            if (enumerator.MoveNext())
            {
                return enumerator.Current;
            }

            throw Logger.Fatal.InvalidOperation(
                SR.EnumeratorExtensions_EnumerationEnded
            );
        }

        public static IEnumerable<T> TakeRemainder<T>(this IEnumerator<T> enumerator)
        {
            if (enumerator == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(enumerator));
            }

            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(EnumeratorExtensions));
    }
}
