using HarshPoint.Provisioning;
using System;
using System.CodeDom;

namespace HarshPoint.ShellployGenerator.Commands
{
    internal sealed class BuildFieldTaxonomy :
        HarshPointCommandBuilder<HarshModifyFieldTaxonomy>
    {
        public BuildFieldTaxonomy()
        {
            AsChildOf<HarshField>(p =>
            {
                p.Parameter(x => x.TypeName).SetFixedValue("TaxonomyFieldType");
            });

            Parameter("TermSetId").Synthesize(typeof(Guid));

            Parameter(x => x.TermSet).SetFixedValue(
                new CodeTypeReferenceExpression(typeof(Resolve))
                    .Call(nameof(Resolve.TermStoreSiteCollectionDefault))
                    .Call(nameof(Resolve.TermSet))
                    .Call(nameof(Resolve.ById), new CodeVariableReferenceExpression("TermSetId"))
            );

        }
    }
}