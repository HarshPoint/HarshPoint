using HarshPoint;
using HarshPoint.Provisioning;
using System;

namespace CommandBuilding
{
    public class TestProvisioner : HarshProvisioner
    {
        [Parameter()]
        public String BasicParam { get; set; }

        [Parameter(Mandatory = true)]
        public String MandatoryParam { get; set; }

        [Parameter(ParameterSetName = "A")]
        public String ParamSetA { get; set; }

        [Parameter(ParameterSetName = "B")]
        public String ParamSetB { get; set; }

        [Parameter(ParameterSetName = "A")]
        [Parameter(ParameterSetName = "B", Mandatory = true)]
        public String ParamSetA_BMandatory { get; set; }
    }
}
