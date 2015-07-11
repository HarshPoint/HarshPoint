namespace HarshPoint.Provisioning
{
    public interface IResolveSingleOrDefault<out T>
    {
        T Value { get; }
    }
}
