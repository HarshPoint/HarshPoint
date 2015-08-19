using HarshPoint.Provisioning;

namespace HarshPoint.ShellployGenerator
{
    internal sealed class HarshContentTypeMetadata :
        HarshPointShellployCommand<HarshContentType>
    {
        public HarshContentTypeMetadata()
        {
            AddPositionalParameter(x => x.Id);
            AddPositionalParameter(x => x.Name);
            HasChildren();
        }
    }
}