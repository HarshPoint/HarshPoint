using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning
{
    public class ContentTypeTests : SharePointClientTest
    {
        private String _guid;
        private HarshContentTypeId _id;

        private const String Group = "HarshPoint Unit Tests";

        public ContentTypeTests(SharePointClientFixture fixture, ITestOutputHelper output) : base(fixture, output)
        {
            _guid = Guid.NewGuid().ToString("n");
            _id = HarshContentTypeId.Parse("0x01").Append(HarshContentTypeId.Parse(_guid));
        }

        [Fact(Skip = "inconclusive")]
        public async Task Existing_content_type_is_not_provisioned()
        {
            var prov = new HarshContentType()
            {
                Id = HarshContentTypeId.Parse("0x01"),
            };

            await prov.ProvisionAsync(Fixture.Context);
            //Assert.False(prov.Result.ObjectAdded);
        }

        [Fact]
        public async Task ContentType_without_parent_gets_provisioned()
        {
            var prov = new HarshContentType()
            {
                Id = _id,
                Name = _guid,
                Description = _guid,
                Group = Group
            };

            try
            {
                await prov.ProvisionAsync(Fixture.Context);
                Assert.NotNull(prov.ContentType);

                Fixture.ClientContext.Load(
                    prov.ContentType,
                    ct => ct.Name,
                    ct => ct.Description,
                    ct => ct.Group,
                    ct => ct.StringId
                );

                await Fixture.ClientContext.ExecuteQueryAsync();

                Assert.Equal(_guid, prov.ContentType.Name);
                Assert.Equal(_guid, prov.ContentType.Description);
                Assert.Equal(Group, prov.ContentType.Group);
                Assert.Equal(_id.ToString(), prov.ContentType.StringId);
            }
            finally
            {
                if (prov.ContentType != null)
                {
                    prov.ContentType.DeleteObject();
                    await Fixture.ClientContext.ExecuteQueryAsync();
                }
            }
        }

        [Fact]
        public async Task Child_fieldref_get_added()
        {
            var fieldId = Guid.NewGuid();

            var field = new HarshField()
            {
                Id = fieldId,
                DisplayName = fieldId.ToString("n"),
                InternalName = fieldId.ToString("n"),
            };

            var ct = new HarshContentType()
            {
                Id = _id,
                Name = _guid,
                Description = _guid,
                Group = Group,
                Children =
                {
                    new HarshFieldRef()
                    {
                        Fields = Resolve.Field.ById(fieldId),
                    },
                }
            };

            try
            {
                await field.ProvisionAsync(Fixture.Context);
                await ct.ProvisionAsync(Fixture.Context);

                Assert.False(ct.ContentType.IsNull());

                var links = Fixture.ClientContext.LoadQuery(
                    ct.ContentType.FieldLinks
                    .Where(fl => fl.Id == fieldId)
                    .Include(
                        fl => fl.Name,
                        fl => fl.Id
                    )
                );

                await Fixture.ClientContext.ExecuteQueryAsync();

                var link = Assert.Single(links);

                Assert.NotNull(link);
                Assert.Equal(fieldId, link.Id);
                Assert.Equal(field.InternalName, link.Name);
            }
            finally
            {
                try
                {
                    Fixture.Web.ContentTypes.GetById(_id.ToString()).DeleteObject();
                }
                finally
                {
                    Fixture.Web.Fields.GetById(fieldId).DeleteObject();
                }
            }
        }

        [Fact]
        public async Task Default_group_is_used()
        {
            var prov = new HarshContentType()
            {
                Id = HarshContentTypeId.Parse("0x010044fbfdb9defa4244831062437d181c6f"),
                Name = "44fbfdb9defa4244831062437d181c6f",
            };

            try
            {
                var ctx = Fixture.Context.PushState(new DefaultContentTypeGroup()
                {
                    Value = Group
                });

                await prov.ProvisionAsync(ctx);

                Assert.NotNull(prov.ContentType);
                Assert.Equal(Group, prov.ContentType.Group);
            }
            finally
            {
                Fixture.Web.ContentTypes.GetById("0x010044fbfdb9defa4244831062437d181c6f").DeleteObject();
                await Fixture.ClientContext.ExecuteQueryAsync();
            }
        }
    }
}
