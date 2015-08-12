using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using System.CodeDom;

namespace HarshPoint.ShellployGenerator
{
    internal sealed class HarshRemoveContentTypeRefMetadata
        : ShellployMetadataObject<HarshRemoveContentTypeRef>
    {
        protected override ShellployCommandBuilder<HarshRemoveContentTypeRef> CreateCommandBuilder()
        {
            return base.CreateCommandBuilder()
                .AddPositionalParameter<HarshContentTypeId[]>(
                    "ContentTypeId",
                    new ShellployCommandPropertyParameterAttribute()
                    {
                        Mandatory = true,
                    }
                )
                .SetParameterValue(x => x.ContentTypes,
                    new CodeTypeReferenceExpression(typeof(Resolve))
                        .Call(nameof(Resolve.ContentType))
                        .Call(nameof(Resolve.ById), new CodeVariableReferenceExpression("ContentTypeId"))
                        .Call(nameof(ResolveBuilderExtensions.As), typeof(ContentType))
                );
        }
    }
}