using HarshPoint.Provisioning;
using HarshPoint.ShellployGenerator.Builders;
using Microsoft.SharePoint.Client;

namespace HarshPoint.ShellployGenerator.Commands
{
    internal sealed class BuildList :
        NewProvisionerCommandBuilder<HarshList>
    {
        public BuildList()
        {
            ProvisionerDefaults.Include(this);

            PositionalParameter(x => x.Url);

            Parameter(x => x.TemplateType)
                .SetDefaultValue(ListTemplateType.GenericList);

            HasInputObject = true;
        }
    }
}