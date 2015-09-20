using System;

namespace HarshPoint.ShellployGenerator.Builders
{
    internal static class PropertyModelValidator
    {
        internal static void ValidateDoesNotContain<T>(
            PropertyModel existing,
            PropertyModel inserted
        )
        {
            if (inserted == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(inserted));
            }

            if (existing != null)
            {
                var existingElement = existing.FirstElementOfType<T>();
                if (existingElement != null)
                {
                    throw Logger.Fatal.InvalidOperationFormat(
                        SR.ParameterBuilder_AttemptedToNest,
                        inserted.GetType().Name,
                        existingElement.GetType().Name
                    );
                }
            }
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(PropertyModelValidator));
    }
}