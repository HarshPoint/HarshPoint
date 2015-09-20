using HarshPoint.Provisioning;
using HarshPoint.ShellployGenerator.Builders;
using Microsoft.SharePoint.Client;
using System;
using System.CodeDom;
using SMA = System.Management.Automation;

namespace HarshPoint.ShellployGenerator.Commands
{
    internal sealed class BuildFieldUser :
        NewProvisionerCommandBuilder<HarshModifyFieldUser>
    {
        private static String GroupByIdParameterSet = "GroupById";
        private static String GroupByNameParameterSet = "GroupByName";

        public BuildFieldUser()
        {
            ProvisionerDefaults.Include(this);

            DefaultParameterSetName = GroupByIdParameterSet;

            AsChildOf<HarshField>(
                p => p.Parameter(x => x.Type).SetFixedValue(FieldType.User)
            );

            PositionalParameter("SelectionGroupId").Synthesize(
                typeof(Nullable<Int32>),
                new AttributeBuilder(typeof(SMA.ParameterAttribute))
                {
                    Properties =
                    {
                        ["ParameterSetName"] = GroupByIdParameterSet,
                    }
                }
            );

            PositionalParameter("SelectionGroupName").Synthesize(
                typeof(String),
                new AttributeBuilder(typeof(SMA.ParameterAttribute))
                {
                    Properties =
                    {
                        ["ParameterSetName"] = GroupByNameParameterSet,
                    }
                }
            );

            Parameter(x => x.SelectionGroup).SetConditionalFixedValue(x =>
            {
                x.When(
                    new CodeVariableReferenceExpression("SelectionGroupId")
                        .IsNotNull(),
                    new CodeTypeReferenceExpression(typeof(Resolve))
                        .Call(nameof(Resolve.SiteGroup))
                        .Call(nameof(Resolve.ById), new CodeCastExpression(
                            typeof(Int32),
                            new CodeVariableReferenceExpression("SelectionGroupId")
                        ))
                );
                x.When(
                    new CodeVariableReferenceExpression("SelectionGroupName")
                        .IsNotNullOrEmpty(),
                    new CodeTypeReferenceExpression(typeof(Resolve))
                        .Call(nameof(Resolve.SiteGroup))
                        .Call(nameof(Resolve.ByName), new CodeVariableReferenceExpression("SelectionGroupName"))
                );
            });
        }
    }
}