using System;
using System.Globalization;

namespace HarshPoint
{
    public static class HarshFormattable
    {
        public static String ToStringInvariant(this IFormattable formattable)
            => ToStringInvariant(formattable, null);

        public static String ToStringInvariant(this IFormattable formattable, String format)
        {
            if (formattable == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(formattable));
            }

            return formattable.ToString(format, CultureInfo.InvariantCulture);
        }

        public static String Invariant(IFormattable formattable)
            => formattable.ToStringInvariant(null);

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(HarshFormattable));
    }
}
