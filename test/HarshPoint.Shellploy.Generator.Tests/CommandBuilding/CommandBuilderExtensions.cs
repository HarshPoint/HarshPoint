using HarshPoint.ShellployGenerator;
using HarshPoint.ShellployGenerator.Builders;

namespace CommandBuilding
{
    internal static class CommandBuilderExtensions
    {
        public static ShellployCommand ToCommand<T>(
            this CommandBuilder<T> builder
        )
            => ((ICommandBuilder)(builder)).ToCommand(
                new CommandBuilderContext()
            );
    }
}
