using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Collections;
using SMA = System.Management.Automation;

namespace HarshPoint.Shellploy
{
    [Cmdlet(VerbsCommon.New, "ContentType")]
    [OutputType(typeof(HarshPoint.Provisioning.HarshContentType))]
    public sealed class ContentType : PSCmdlet
    {
        [SMA.Parameter(Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public HarshContentTypeId Id { get; set; }

        [SMA.Parameter(Position = 1, Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public String Name { get; set; }

        [SMA.Parameter(Position = 2, ValueFromPipelineByPropertyName = true)]
        public ScriptBlock Children { get; set; }

        [SMA.Parameter(ValueFromPipelineByPropertyName = true)]
        public String Group { get; set; }

        protected override void ProcessRecord()
        {
            var result = new Provisioning.HarshContentType
            {
                Id = Id,
                Name = Name,
                Group = Group,
            };

            if (Children != null)
            {
                var psChildren = Children.Invoke();
                foreach (var child in psChildren)
                {
                    HarshPoint.Provisioning.Implementation.HarshProvisionerTreeBuilder.AddChild(result, child.BaseObject);
                }
            }
            WriteObject(result);
        }
    }
}
