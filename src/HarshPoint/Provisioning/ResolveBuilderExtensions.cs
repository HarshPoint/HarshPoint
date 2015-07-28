using HarshPoint.Provisioning.Implementation;

namespace HarshPoint.Provisioning
{
    public static class ResolveBuilderExtensions
    {
        public static IResolveBuilder<TResult> As<TResult>(this IResolveBuilder builder)
        {
            if (builder == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(builder));
            }

            return new ResolveBuilderAdapter<TResult>(builder);
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ResolveBuilderExtensions));
    }
}
