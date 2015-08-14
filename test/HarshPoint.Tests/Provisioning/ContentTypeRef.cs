using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Output;
using Microsoft.SharePoint.Client;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning
{
    public class ContentTypeRef : SharePointClientTest
    {
        public ContentTypeRef(ITestOutputHelper output) : base(output)
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

            await ctProv.ProvisionAsync(Context);

            var ctResult = LastObjectOutput<ContentType>();
            var ct = ctResult.Object;
            RegisterForDeletion(ct);

            await listProv.ProvisionAsync(Context);

            var listResult = LastObjectOutput<List>();
            var list = listResult.Object;
            RegisterForDeletion(list);

            Assert.IsType<ObjectAdded<List>>(listResult);
            Assert.NotNull(list);

            ClientContext.Load(
                list,
                l => l.ContentTypesEnabled,
                l => l.ContentTypes.Include(lct => lct.StringId),
                l => l.Id
            );

            await ClientContext.ExecuteQueryAsync();

            Assert.True(list.ContentTypesEnabled);
            Assert.Contains(list.ContentTypes, lct => lct.StringId.StartsWith(ctid.ToString() + "00"));

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
            RegisterForDeletion(ct);

            var list = await CreateList();

            var listCt = list.ContentTypes.AddExistingContentType(ct);
            ClientContext.Load(listCt, c => c.StringId);

            await ClientContext.ExecuteQueryAsync();

            var prov = new HarshContentTypeRef()
            {
                ContentTypes = Resolve.ContentType().ById(HarshContentTypeId.Parse(ctid)),
                Lists = Resolve.List().ById(list.Id),
            };

            await prov.UnprovisionAsync(
                Context.AllowDeleteUserData()
            );

            var actualListCts = ClientContext.LoadQuery(
                list.ContentTypes.Include(c => c.StringId)
            );

            await ClientContext.ExecuteQueryAsync();

            Assert.DoesNotContain(
                listCt.StringId,
                actualListCts.Select(c => c.StringId),
                StringComparer.InvariantCultureIgnoreCase
            );
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

            RegisterForDeletion(ct);

            var list = await CreateList();

            var listCt = list.ContentTypes.AddExistingContentType(ct);
            ClientContext.Load(listCt, c => c.StringId);

            var actualListCts = ClientContext.LoadQuery(
                list.ContentTypes.Include(c => c.StringId)
            );

            await ClientContext.ExecuteQueryAsync();

            Assert.Contains(actualListCts, c => c.StringId == listCt.StringId);

            var prov = new HarshRemoveContentTypeRef()
            {
                ContentTypes = Resolve.ContentType().ById(HarshContentTypeId.Parse(ctid)),
                Lists = Resolve.List().ById(list.Id),
            };

            await prov.ProvisionAsync(Context);

            actualListCts = ClientContext.LoadQuery(
                list.ContentTypes.Include(c => c.StringId)
            );

            await ClientContext.ExecuteQueryAsync();

            Assert.DoesNotContain(actualListCts, c => c.StringId == listCt.StringId);
        }
    }
}
