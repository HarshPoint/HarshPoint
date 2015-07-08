namespace HarshPoint.Provisioning
{
    public interface IResolveSingle<out T>
    {
        T Result { get; }
    }
}
