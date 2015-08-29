using HarshPoint.Provisioning;
using HarshPoint.ShellployGenerator.Builders;
using Microsoft.SharePoint.Client;

namespace HarshPoint.ShellployGenerator.Commands
{
    internal sealed class BuildFieldMultilineText :
        NewProvisionerCommandBuilder<HarshModifyFieldMultilineText>
    {
        public BuildFieldMultilineText()
        {
            ProvisionerDefaults.Include(this);

            AsChildOf<HarshField>(p =>
            {
                p.Parameter(x => x.Type).SetFixedValue(FieldType.Note);
            });
        }
    }
}