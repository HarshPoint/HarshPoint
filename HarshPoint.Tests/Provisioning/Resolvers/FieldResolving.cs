using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using System;
using System.Threading.Tasks;
using Xunit;

namespace HarshPoint.Tests.Provisioning.Resolvers
{
    public class FieldResolving : IClassFixture<SharePointClientFixture>
    {
        public FieldResolving(SharePointClientFixture fixture)
        {
            Fixture = fixture;
        }

        public SharePointClientFixture Fixture { get; private set; }

        [Fact]
        public async Task Documents_Title_field_gets_resolved_by_id()
        {
            IResolveSingle<Field> resolver = Resolve
                .ListByUrl("Shared Documents")
                .FieldById(HarshBuiltInFieldId.Title);

            var field = await resolver.ResolveSingleAsync(Fixture.Context);

            Assert.NotNull(field);
            Assert.Equal("Title", field.InternalName);
        }

        private async Task EnsureDocuments()
        {
            var list = Fixture.Web.Lists.GetByTitle("Documents");
            Fixture.ClientContext.Load(list);

            await Fixture.ClientContext.ExecuteQueryAsync();

            if (list.IsNull())
            {
                list = Fixture.Web.Lists.Add(new ListCreationInformation()
                {
                    Url = "Shared%20Documents",
                    Title = "Documents",
                    TemplateType = (Int32)ListTemplateType.DocumentLibrary,
                    DocumentTemplateType = (Int32)DocumentTemplateType.Word,
                });

                await Fixture.ClientContext.ExecuteQueryAsync();
            }

        }
    }
}
