using HarshPoint.Provisioning;
using HarshPoint.ShellployGenerator.Builders;
using Microsoft.SharePoint.Client;

namespace HarshPoint.ShellployGenerator.Commands
{
    internal sealed class BuildFieldUser :
        NewProvisionerCommandBuilder<HarshModifyFieldUser>
    {
        public BuildFieldUser()
        {
            ProvisionerDefaults.Include(this);

            AsChildOf<HarshField>(
                p => p.Parameter(x => x.Type).SetFixedValue(FieldType.User)
            );
        }
    }
}