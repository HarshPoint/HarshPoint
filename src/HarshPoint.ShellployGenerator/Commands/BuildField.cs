using HarshPoint.Provisioning;

namespace HarshPoint.ShellployGenerator.Commands
{
    internal sealed class BuildField :
        HarshPointCommandBuilder<HarshModifyField>
    {
        public BuildField()
        {
            AsChildOf<HarshField>();
        }
    }
}