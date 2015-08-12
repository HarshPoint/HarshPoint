using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using System;
using System.CodeDom;

namespace HarshPoint.ShellployGenerator
{
    internal sealed class HarshFieldRefMetadata
        : ShellployMetadataObject<HarshFieldRef>
    {
        protected override ShellployCommandBuilder<HarshFieldRef> CreateCommandBuilder()
        {
            return base.CreateCommandBuilder()
                .AddNamedParameter<String>("InternalName")
                .SetParameterValue(x => x.Fields,
                    new CodeTypeReferenceExpression(typeof(Resolve))
                        .Call(nameof(Resolve.Field))
                        .Call(nameof(Resolve.ByInternalName), new CodeVariableReferenceExpression("InternalName"))
                        .Call(nameof(ResolveBuilderExtensions.As), typeof(Field))
                );
        }
    }
}