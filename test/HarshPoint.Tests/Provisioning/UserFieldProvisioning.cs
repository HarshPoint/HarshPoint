using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning
{
    public class UserFieldProvisioning : TestFieldBase<FieldUser, HarshModifyFieldUser>
    {
        public UserFieldProvisioning(ITestOutputHelper output)
            : base(FieldType.User, output)
        {
        }

        [FactNeedsSharePoint]
        public async Task SelectionMode_is_set()
        {
            var prov = new HarshModifyFieldUser()
            {
                SelectionMode = FieldUserSelectionMode.PeopleAndGroups,
            };

            await RunWithField(prov, f =>
            {
                Assert.Equal(FieldUserSelectionMode.PeopleAndGroups, f.SelectionMode);
            });
        }

        [FactNeedsSharePoint]
        public async Task SelectionGroup_is_set()
        {
            var group = await CreateSiteGroup();

            var prov = new HarshModifyFieldUser()
            {
                SelectionGroup = Resolve.SiteGroup().ById(group.Id),
            };

            await RunWithField(prov, f =>
            {
                Assert.Equal(group.Id, f.SelectionGroup);
            });
        }

        [FactNeedsSharePoint]
        public async Task SelectionGroup_is_set_empty()
        {
            var group = await CreateSiteGroup();

            var prov = new HarshModifyFieldUser();

            await RunWithField(prov, f =>
            {
                Assert.Equal(-1, f.SelectionGroup);
            });
        }
    }
}
