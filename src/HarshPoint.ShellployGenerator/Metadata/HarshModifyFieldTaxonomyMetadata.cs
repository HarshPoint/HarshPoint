using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client.Taxonomy;
using System;
using System.CodeDom;

namespace HarshPoint.ShellployGenerator
{
    internal sealed class HarshModifyFieldTaxonomyMetadata :
        HarshPointShellployCommand<HarshModifyFieldTaxonomy>
    {
        public HarshModifyFieldTaxonomyMetadata()
        {
            AsChildOf<HarshField>(p =>
            {
                p.SetFixedValue(x => x.TypeName, "TaxonomyFieldType");
                p.Ignore(x => x.Type);
            });

            Parameter(x => x.Fields).Rename("Field");

            Parameter("TermSetId").Synthesize(typeof(Guid));

            Parameter(x => x.TermSet).SetFixedValue(
                new CodeTypeReferenceExpression(typeof(Resolve))
                    .Call(nameof(Resolve.TermStoreSiteCollectionDefault))
                    .Call(nameof(Resolve.TermSet))
                    .Call(nameof(Resolve.ById), new CodeVariableReferenceExpression("TermSetId"))
                    .Call(nameof(ResolveBuilderExtensions.As), typeof(TermSet))
            );

        }
    }
}