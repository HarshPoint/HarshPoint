using HarshPoint.Provisioning;
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

            var ct = new HarshContentType()
            {
                Id = ctid,
                Name = name,
            };

            var list = new HarshList()
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

            try
            {
                await ct.ProvisionAsync(Fixture.Context);

                Assert.NotNull(ct.ContentType);

                await list.ProvisionAsync(Fixture.Context);

                Assert.True(list.ListAdded);
                Assert.NotNull(list.List);

                Fixture.ClientContext.Load(
                    list.List,
                    l => l.ContentTypesEnabled,
                    l => l.ContentTypes.Include(lct => lct.StringId),
                    l => l.Id
                );

                await Fixture.ClientContext.ExecuteQueryAsync();

                Assert.True(list.List.ContentTypesEnabled);
                Assert.Contains(list.List.ContentTypes, lct => lct.StringId.StartsWith(ctid.ToString() + "00"));
            }
            finally
            {
                if (list.List != null)
                {
                    list.List.DeleteObject();
                    await Fixture.ClientContext.ExecuteQueryAsync();
                }

                if (ct.ContentType != null)
                {
                    ct.ContentType.DeleteObject();
                    await Fixture.ClientContext.ExecuteQueryAsync();
                }
            }
        }
    }
}
