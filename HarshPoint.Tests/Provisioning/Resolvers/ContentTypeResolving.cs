using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using System;
using System.Threading.Tasks;
using Xunit;

namespace HarshPoint.Tests.Provisioning.Resolvers
{
    public class ContentTypeResolving : IClassFixture<SharePointClientFixture>
    {
        public ContentTypeResolving(SharePointClientFixture fix)
        {
            Fixture = fix;
        }

        public SharePointClientFixture Fixture { get; private set; }

        [Theory]
        [InlineData("0x01")]
        [InlineData("0x0101")]
        [InlineData("0x0120")]
        public async Task Valid_id_gets_resolved(String id)
        {
            IResolveSingle<ContentType> resolver = Resolve.ContentTypeById(id);
            var ct = await resolver.ResolveSingleOrDefaultAsync(Fixture.Context);

            Assert.False(ct.IsNull());
            Assert.Equal(id, await ct.EnsurePropertyAvailable(c => c.StringId));
        }
    }
}
