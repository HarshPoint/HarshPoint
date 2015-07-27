namespace HarshPoint.Provisioning.Implementation
{
    public interface IResolveBuilder<out TResult, TContext> :
        IResolveBuilder<TContext>,
        IResolve<TResult>,
        IResolveSingle<TResult>,
        IResolveSingleOrDefault<TResult>
        where TContext : IResolveContext
    {
    }
}
