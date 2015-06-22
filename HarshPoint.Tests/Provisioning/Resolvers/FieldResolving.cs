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
            await Fixture.EnsureTestList();

            IResolve<Field> resolver = Resolve
                .ListByUrl(SharePointClientFixture.TestListUrl)
                .FieldById(HarshBuiltInFieldId.Title);

            var field = Assert.Single(
                await resolver.TryResolveAsync(
                    Fixture.ResolveContext
                )
            );

            Assert.NotNull(field);
            Assert.Equal("Title", await field.EnsurePropertyAvailable(f => f.InternalName));
        }
    }
}
