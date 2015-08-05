using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Output;
using Microsoft.SharePoint.Client;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning
{
    public class ContentTypeRef : SharePointClientTest
    {
        public ContentTypeRef(SharePointClientFixture fixture, ITestOutputHelper output) : base(fixture, output)
        {
        }

        [Fact]
        public async Task Content_type_gets_added()
        {
            var name = Guid.NewGuid().ToString("n");
            var ctid = HarshContentTypeId.Parse("0x01").Append(HarshContentTypeId.Parse(name));

            var ctProv = new HarshContentType()
            {
                Id = ctid,
                Name = name,
            };

            var listProv = new HarshList()
            {
                Title = name,
                Url = "Lists/" + name,

                Children =
                {
                    new HarshContentTypeRef()
                    {
                        ContentTypes = Resolve.ContentType().ById(ctid)
                    }
                }
            };

            ContentType ct = null;
            List list = null;

            try
            {
                await ctProv.ProvisionAsync(Fixture.Context);

                var ctResult = FindOutput<ContentType>();
                ct = ctResult.Object;

                await listProv.ProvisionAsync(Fixture.Context);

                var listResult = FindOutput<List>();

                list = listResult.Object;

                Assert.IsType<ObjectCreated<List>>(listResult);
                Assert.NotNull(list);

                Fixture.ClientContext.Load(
                    list,
                    l => l.ContentTypesEnabled,
                    l => l.ContentTypes.Include(lct => lct.StringId),
                    l => l.Id
                );

                await Fixture.ClientContext.ExecuteQueryAsync();

                Assert.True(list.ContentTypesEnabled);
                Assert.Contains(list.ContentTypes, lct => lct.StringId.StartsWith(ctid.ToString() + "00"));
            }
            finally
            {
                if (list != null)
                {
                    list.DeleteObject();
                    await Fixture.ClientContext.ExecuteQueryAsync();
                }

                if (ct != null)
                {
                    ct.DeleteObject();
                    await Fixture.ClientContext.ExecuteQueryAsync();
                }
            }
        }
    }
}
