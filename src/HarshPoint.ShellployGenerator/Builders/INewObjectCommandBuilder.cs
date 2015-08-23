namespace HarshPoint.ShellployGenerator.Builders
{
    internal interface INewObjectCommandBuilder<TTarget>
    {
        PropertyModelContainer PropertyContainer { get; }
    }
}