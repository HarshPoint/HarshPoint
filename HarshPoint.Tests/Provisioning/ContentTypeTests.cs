using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace HarshPoint.Tests.Provisioning
{
    public class ContentTypeTests : IClassFixture<SharePointClientFixture>
    {
        private String _guid;
        private HarshContentTypeId _id;

        private const String Group = "HarshPoint Unit Tests";

        public ContentTypeTests(SharePointClientFixture fix)
        {
            Fixture = fix;

            _guid = Guid.NewGuid().ToString("n");
            _id = HarshContentTypeId.Parse("0x01").Append(HarshContentTypeId.Parse(_guid));
        }

        public SharePointClientFixture Fixture { get; private set; }

        [Fact]
        public async Task Existing_content_type_is_not_provisioned()
        {
            var prov = new HarshContentType()
            {
                Id = HarshContentTypeId.Parse("0x01"),
            };

            await prov.ProvisionAsync(Fixture.Context);

            Assert.False(prov.ContentTypeAdded);
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

                Assert.True(prov.ContentTypeAdded);
                Assert.False(prov.ContentType.IsNull());

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
                ;
            }
            finally
            {
                if (!prov.ContentType.IsNull())
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
                        Fields = Resolve.FieldById(fieldId),
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
                if (ct.Result.ObjectAdded)
                {
                    ct.Result.Object.DeleteObject();
                    await Fixture.ClientContext.ExecuteQueryAsync();
                }

                if (field.Result.ObjectAdded)
                {
                    field.Result.Object.DeleteObject();
                    await Fixture.ClientContext.ExecuteQueryAsync();
                }
            }
        }
    }
}
