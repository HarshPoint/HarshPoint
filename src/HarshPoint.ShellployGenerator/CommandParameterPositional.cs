using System;

namespace HarshPoint.ShellployGenerator
{
    internal sealed class CommandParameterPositional : CommandParameter
    {
        public CommandParameterPositional(Int32 sortOrder)
        {
            SortOrder = sortOrder;
        }

        internal override void Process(ShellployCommandProperty property)
        {
            property.IsPositional = true;
        }
    }
}
