using HarshPoint.Provisioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HarshPoint.Tests.Provisioning
{
    public class ContentTypeTests : IClassFixture<SharePointClientFixture>
    {
        public ContentTypeTests(SharePointClientFixture fix)
        {
            Fixture = fix;
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
            var guid = Guid.NewGuid().ToString("n");
            var group = "HarshPoint Unit Tests";

            var prov = new HarshContentType()
            {
                Name = guid,
                Description = guid,
                Group = group
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
                    ct => ct.Group
                );

                await Fixture.ClientContext.ExecuteQueryAsync();

                Assert.Equal(guid, prov.ContentType.Name);
                Assert.Equal(guid, prov.ContentType.Description);
                Assert.Equal(group, prov.ContentType.Group);
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
    }
}
