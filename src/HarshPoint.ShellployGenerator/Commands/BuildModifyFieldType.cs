using HarshPoint.Provisioning;
using HarshPoint.ShellployGenerator.Builders;

namespace HarshPoint.ShellployGenerator.Commands
{
    internal sealed class BuildModifyFieldType : 
        NewProvisionerCommandBuilder<HarshModifyFieldType>
    {
        public BuildModifyFieldType()
        {
            ProvisionerDefaults.Include(this);

            AsChildOf<HarshField>(f=>
            {
                f.Parameter(x => x.Type).Ignore();
                f.Parameter(x => x.TypeName).Ignore();
            });
        }
    }
}
