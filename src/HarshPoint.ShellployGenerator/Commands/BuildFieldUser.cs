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
        private static String SelectionGroupByIdParameterSet = "SelectionGroupById";
        private static String SelectionGroupByNameParameterSet = "SelectionGroupByName";

        public BuildFieldUser()
        {
            ProvisionerDefaults.Include(this);

            DefaultParameterSetName = SelectionGroupByIdParameterSet;

            AsChildOf<HarshField>(
                p => p.Parameter(x => x.Type).SetFixedValue(FieldType.User)
            );

            PositionalParameter("SelectionGroupId").Synthesize(
                typeof(Nullable<Int32>),
                new AttributeBuilder(typeof(SMA.ParameterAttribute))
                {
                    Properties =
                    {
                        ["ParameterSetName"] = SelectionGroupByIdParameterSet,
                    }
                }
            );

            PositionalParameter("SelectionGroupName").Synthesize(
                typeof(String),
                new AttributeBuilder(typeof(SMA.ParameterAttribute))
                {
                    Properties =
                    {
                        ["ParameterSetName"] = SelectionGroupByNameParameterSet,
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