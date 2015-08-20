using HarshPoint.Provisioning;

namespace HarshPoint.ShellployGenerator
{
    internal sealed class HarshContentTypeMetadata :
        HarshPointShellployCommand<HarshContentType>
    {
        public HarshContentTypeMetadata()
        {
            PositionalParameter(x => x.Id);
            PositionalParameter(x => x.Name);
            HasInputObject = true;
        }
    }
}