using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using System.CodeDom;

namespace HarshPoint.ShellployGenerator
{
    internal sealed class HarshRemoveContentTypeRefMetadata :
        HarshPointShellployCommand<HarshRemoveContentTypeRef>
    {
        public HarshRemoveContentTypeRefMetadata()
        {
            AddPositionalParameter<HarshContentTypeId[]>(
                "ContentTypeId",
                new ShellployCommandPropertyParameterAttribute()
                {
                    Mandatory = true,
                }
            );
            RenameParameter(x => x.Lists, "List");
            SetParameterValue(x => x.ContentTypes,
                new CodeTypeReferenceExpression(typeof(Resolve))
                    .Call(nameof(Resolve.ContentType))
                    .Call(nameof(Resolve.ById), new CodeVariableReferenceExpression("ContentTypeId"))
                    .Call(nameof(ResolveBuilderExtensions.As), typeof(ContentType))
            );
        }
    }
}