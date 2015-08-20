using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;

namespace HarshPoint.ShellployGenerator
{
    internal sealed class HarshListMetadata :
        HarshPointShellployCommand<HarshList>
    {
        public HarshListMetadata()
        {
            PositionalParameter(x => x.Title);
            PositionalParameter(x => x.Url);

            Parameter(x => x.TemplateType)
                .SetDefaultValue(ListTemplateType.GenericList);

            HasInputObject = true;
        }
    }
}