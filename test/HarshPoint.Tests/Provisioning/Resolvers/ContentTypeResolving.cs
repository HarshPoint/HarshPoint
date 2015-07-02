using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning.Resolvers
{
    public class ContentTypeResolving : SharePointClientTest
    {
        public ContentTypeResolving(SharePointClientFixture fixture, ITestOutputHelper output) 
            : base(fixture, output)
        {
        }

        [Theory]
        [InlineData("0x01")]
        [InlineData("0x0101")]
        [InlineData("0x0120")]
        public async Task Valid_id_gets_resolved(String id)
        {
            IResolve<ContentType> resolver = Resolve.ContentTypeById(id);
            var ct = Assert.Single(await resolver.TryResolveAsync(Fixture.ResolveContext));

            Assert.False(ct.IsNull());
            Assert.Equal(id, await ct.EnsurePropertyAvailable(c => c.StringId));
        }
    }
}
