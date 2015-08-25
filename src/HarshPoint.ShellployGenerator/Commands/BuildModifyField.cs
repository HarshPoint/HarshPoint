using HarshPoint.Provisioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarshPoint.ShellployGenerator.Commands
{
    internal sealed class BuildModifyField : HarshPointCommandBuilder<HarshModifyFieldType>
    {
        public BuildModifyField()
        {
            AsChildOf<HarshField>();
        }
    }
}
