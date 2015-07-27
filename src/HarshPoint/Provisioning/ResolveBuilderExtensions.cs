using HarshPoint.Provisioning.Implementation;

namespace HarshPoint.Provisioning
{
    public static class ResolveBuilderExtensions
    {
        public static ResolveBuilderAdapter<T> As<T>(this IResolveBuilder builder)
        {
            if (builder == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(builder));
            }

            return new ResolveBuilderAdapter<T>(builder);
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ResolveBuilderExtensions));
    }
}
