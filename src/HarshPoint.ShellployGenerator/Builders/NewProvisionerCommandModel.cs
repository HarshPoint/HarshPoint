using System.Collections.Generic;
using System.Collections.Immutable;

namespace HarshPoint.ShellployGenerator.Builders
{
    public sealed class NewProvisionerCommandModel
    {
        public NewProvisionerCommandModel(
            CommandModel command,
            IEnumerable<NewObjectCommandModel> newObjects
        )
        {
            Command = command;
            NewObjects = newObjects.ToImmutableArray();
        }

        public CommandModel Command { get; }
        public ImmutableArray<NewObjectCommandModel> NewObjects { get; }
    }
}
