using System;
using System.Diagnostics.CodeAnalysis;

namespace HarshPoint
{
    public static class HarshLazy
    {
        [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#")]
        public static T Initialize<T>(ref T field)
            where T : class, new()
        {
            if (field == null)
            {
                field = new T();
            }

            return field;
        }

        [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#")]
        public static T Initialize<T>(ref T field, Func<T> factory)
        {
            if (factory == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(factory));
            }

            if (Equals(field, default(T)))
            {
                field = factory();
            }

            return field;
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(HarshLazy));
    }
}
