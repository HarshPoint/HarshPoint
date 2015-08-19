using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;

namespace HarshPoint.ShellployGenerator
{
    internal sealed class HarshListMetadata :
        HarshPointShellployCommand<HarshList>
    {
        public HarshListMetadata()
        {
            AddPositionalParameter(x => x.Title);
            AddPositionalParameter(x => x.Url);
            SetDefaultParameterValue(x => x.TemplateType, ListTemplateType.GenericList);
            HasChildren();
        }
    }
}