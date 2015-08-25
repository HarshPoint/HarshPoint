using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;

namespace HarshPoint.ShellployGenerator.Commands
{
    internal sealed class BuildList :
        HarshPointCommandBuilder<HarshList>
    {
        public BuildList()
        {
            PositionalParameter(x => x.Url);

            Parameter(x => x.TemplateType)
                .SetDefaultValue(ListTemplateType.GenericList);

            HasInputObject = true;
        }
    }
}