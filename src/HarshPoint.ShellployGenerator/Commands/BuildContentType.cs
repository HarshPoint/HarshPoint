using HarshPoint.Provisioning;

namespace HarshPoint.ShellployGenerator.Commands
{
    internal sealed class BuildContentType :
        HarshPointCommandBuilder<HarshContentType>
    {
        public BuildContentType()
        {
            PositionalParameter(x => x.Id);
            PositionalParameter(x => x.Name);
            HasInputObject = true;
        }
    }
}