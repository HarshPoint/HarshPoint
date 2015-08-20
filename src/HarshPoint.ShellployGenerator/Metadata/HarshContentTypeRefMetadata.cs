using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using System.CodeDom;

namespace HarshPoint.ShellployGenerator
{
    internal sealed class HarshContentTypeRefMetadata :
        HarshPointShellployCommand<HarshContentTypeRef>
    {
        public HarshContentTypeRefMetadata()
        {
            PositionalParameter("ContentTypeId").SynthesizeMandatory(
                typeof(HarshContentTypeId[])
            );

            Parameter(x => x.Lists).Rename("List");

            Parameter(x => x.ContentTypes).SetFixedValue(
                new CodeTypeReferenceExpression(typeof(Resolve))
                    .Call(nameof(Resolve.ContentType))
                    .Call(nameof(Resolve.ById), new CodeVariableReferenceExpression("ContentTypeId"))
                    .Call(nameof(ResolveBuilderExtensions.As), typeof(ContentType))
            );
        }
    }
}