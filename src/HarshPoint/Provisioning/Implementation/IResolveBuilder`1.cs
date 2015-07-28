namespace HarshPoint.Provisioning.Implementation
{
    public interface IResolveBuilder<out TResult> :
        IResolveBuilder,
        IResolve<TResult>,
        IResolveSingle<TResult>,
        IResolveSingleOrDefault<TResult>
    {
    }
}
