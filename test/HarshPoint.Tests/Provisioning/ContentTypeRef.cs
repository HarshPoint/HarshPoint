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
                await ctProv.ProvisionAsync(Context);

                var ctResult = FindOutput<ContentType>();
                ct = ctResult.Object;

                await listProv.ProvisionAsync(Context);

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

        [Fact]
        public async Task Content_type_gets_removed()
        {
            var guid = Guid.NewGuid().ToString("n");
            var ctid = $"0x0100{guid}";

            var ct = Web.ContentTypes.Add(new ContentTypeCreationInformation()
            {
                Id = ctid,
                Name = guid,
                Group = "HarshPoint"
            });

            await ClientContext.ExecuteQueryAsync();

            var list = await Fixture.EnsureTestList();

            var listCt = list.ContentTypes.AddExistingContentType(ct);
            ClientContext.Load(listCt, c => c.StringId);

            await ClientContext.ExecuteQueryAsync();

            try
            {
                var prov = new HarshContentTypeRef()
                {
                    ContentTypes = Resolve.ContentType().ById(HarshContentTypeId.Parse(ctid)),
                    Lists = Resolve.List().ByUrl(SharePointClientFixture.TestListUrl),
                };

                await prov.UnprovisionAsync(
                    Context.AllowDeleteUserData()
                );

                var actualListCts = ClientContext.LoadQuery(list.ContentTypes);
                await ClientContext.ExecuteQueryAsync();

                Assert.DoesNotContain(actualListCts, c => c.StringId == listCt.StringId);
            }
            finally
            {
                list.DeleteObject();
                await ClientContext.ExecuteQueryAsync();

                ct.DeleteObject();
                await ClientContext.ExecuteQueryAsync();
            }
        }

        [Fact]
        public async Task Content_type_gets_removed_using_RemoveContentTypeRef()
        {
            var guid = Guid.NewGuid().ToString("n");
            var ctid = $"0x0100{guid}";

            var ct = Web.ContentTypes.Add(new ContentTypeCreationInformation()
            {
                Id = ctid,
                Name = guid,
                Group = "HarshPoint"
            });

            await ClientContext.ExecuteQueryAsync();

            var list = await Fixture.EnsureTestList();

            var listCt = list.ContentTypes.AddExistingContentType(ct);
            ClientContext.Load(listCt, c => c.StringId);

            await ClientContext.ExecuteQueryAsync();

            try
            {
                var actualListCts = ClientContext.LoadQuery(
                    list.ContentTypes.Include(c => c.StringId)
                );

                await ClientContext.ExecuteQueryAsync();

                Assert.Contains(actualListCts, c => c.StringId == listCt.StringId);

                var prov = new HarshRemoveContentTypeRef()
                {
                    ContentTypes = Resolve.ContentType().ById(HarshContentTypeId.Parse(ctid)),
                    Lists = Resolve.List().ByUrl(SharePointClientFixture.TestListUrl),
                };

                await prov.ProvisionAsync(Context);

                actualListCts = ClientContext.LoadQuery(
                    list.ContentTypes.Include(c => c.StringId)
                );

                await ClientContext.ExecuteQueryAsync();

                Assert.DoesNotContain(actualListCts, c => c.StringId == listCt.StringId);
            }
            finally
            {
                list.DeleteObject();
                await ClientContext.ExecuteQueryAsync();

                ct.DeleteObject();
                await ClientContext.ExecuteQueryAsync();
            }
        }
    }
}
