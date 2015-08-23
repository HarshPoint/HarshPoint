using System;

namespace HarshPoint.ShellployGenerator.Builders
{
    public sealed class NewObjectCommandModel
    {
        public NewObjectCommandModel(
            CommandModel command,
            Type targetType
        )
        {
            Command = command;
            TargetType = targetType;
        }

        public CommandModel Command { get; }
        public Type TargetType { get; }
    }
}
