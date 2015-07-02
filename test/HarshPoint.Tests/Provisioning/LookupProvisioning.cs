using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning
{
    public class LookupProvisioning : SharePointClientTest
    {
        private const String TargetListUrl = "Lists/2c6280a9273e441abecf2909379712be";

        public LookupProvisioning(SharePointClientFixture fixture, ITestOutputHelper output) 
            : base(fixture, output)
        {
        }

        [Fact]
        public async Task Lookup_has_correct_list_id()
        {
            var targetList = await EnsureTargetList();

            var lookupField = new HarshLookupField()
            {
                LookupTarget = Resolve
                .ListByUrl(TargetListUrl)
                .FieldById(HarshBuiltInFieldId.Title)
            };

            var fieldId = Guid.NewGuid();
            var field = new HarshField()
            {
                Id = fieldId,
                InternalName = fieldId.ToString("n"),
                DisplayName = "HarshLookupField",
                Type = FieldType.Lookup,

                Children =
                {
                    lookupField
                }
            };

            await field.ProvisionAsync(Fixture.Context);

            Fixture.ClientContext.Load(
                Fixture.ClientContext.CastTo<FieldLookup>(field.Field),
                f => f.FieldTypeKind,
                f => f.LookupField,
                f => f.LookupList,
                f => f.LookupWebId
            );

            Fixture.ClientContext.Load(Fixture.Web, w => w.Id);
            Fixture.ClientContext.Load(targetList, l => l.Id);

            await Fixture.ClientContext.ExecuteQueryAsync();

            var provisioned = field.Field as FieldLookup;

            Assert.NotNull(provisioned);
            Assert.Equal(FieldType.Lookup, provisioned.FieldTypeKind);
            Assert.Equal(targetList.Id, Guid.Parse(provisioned.LookupList));
            Assert.Equal("Title", provisioned.LookupField);
            Assert.Equal(Fixture.Web.Id, provisioned.LookupWebId);
        }

        private async Task<List> EnsureTargetList()
        {
            var list = new HarshList()
            {
                Title = "HarshPoint Tests Lookup Target List",
                Url = TargetListUrl,
            };

            await list.ProvisionAsync(Fixture.Context);

            return list.List;
        }
    }
}
