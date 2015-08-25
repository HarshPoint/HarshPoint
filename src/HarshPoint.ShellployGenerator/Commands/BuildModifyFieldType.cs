using HarshPoint.Provisioning;

namespace HarshPoint.ShellployGenerator.Commands
{
    internal sealed class BuildModifyFieldType : 
        HarshPointCommandBuilder<HarshModifyFieldType>
    {
        public BuildModifyFieldType()
        {
            AsChildOf<HarshField>(f=>
            {
                f.Parameter(x => x.Type).Ignore();
                f.Parameter(x => x.TypeName).Ignore();
            });
        }
    }
}
