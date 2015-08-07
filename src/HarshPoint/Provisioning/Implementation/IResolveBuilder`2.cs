namespace HarshPoint.Provisioning.Implementation
{
    public interface IResolveBuilder<out TResult, TContext> :
        IResolveBuilder<TResult>,
        IResolve<TResult>,
        IResolveSingle<TResult>,
        IResolveSingleOrDefault<TResult>
        where TContext : IResolveContext
    {
    }
}
