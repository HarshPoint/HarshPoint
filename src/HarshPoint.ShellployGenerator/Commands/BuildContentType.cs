using HarshPoint.Provisioning;
using HarshPoint.ShellployGenerator.Builders;

namespace HarshPoint.ShellployGenerator.Commands
{
    internal sealed class BuildContentType :
        NewProvisionerCommandBuilder<HarshContentType>
    {
        public BuildContentType()
        {
            ProvisionerDefaults.Include(this);

            PositionalParameter(x => x.Id);
            PositionalParameter(x => x.Name);
            HasInputObject = true;
        }
    }
}