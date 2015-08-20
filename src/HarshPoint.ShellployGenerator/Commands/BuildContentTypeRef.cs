using HarshPoint.Provisioning;
using System.CodeDom;

namespace HarshPoint.ShellployGenerator.Commands
{
    internal sealed class BuildContentTypeRef :
        HarshPointCommandBuilder<HarshContentTypeRef>
    {
        public BuildContentTypeRef()
        {
            PositionalParameter("ContentTypeId").SynthesizeMandatory(
                typeof(HarshContentTypeId[])
            );

            Parameter(x => x.ContentTypes).SetFixedValue(
                new CodeTypeReferenceExpression(typeof(Resolve))
                    .Call(nameof(Resolve.ContentType))
                    .Call(nameof(Resolve.ById), new CodeVariableReferenceExpression("ContentTypeId"))
            );
        }
    }
}