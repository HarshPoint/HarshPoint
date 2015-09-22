using HarshPoint.Provisioning;
using HarshPoint.ShellployGenerator.Builders;
using Microsoft.SharePoint.Client;

namespace HarshPoint.ShellployGenerator.Commands
{
    internal sealed class BuildSiteGroup :
        NewProvisionerCommandBuilder<HarshSiteGroup>
    {
        public BuildSiteGroup()
        {
            ProvisionerDefaults.Include(this);

            PositionalParameter(x => x.Name);
            PositionalParameter(x => x.Description);
        }
    }
}