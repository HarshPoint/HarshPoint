using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using Moq;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning.Implementation
{
    public class ClientObjectResolveContextBuilding : SharePointClientTest
    {
        public ClientObjectResolveContextBuilding(SharePointClientFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
        }

        [Fact]
        public void Retrievals_get_recorded_by_the_resolved_type()
        {
            var builder = new ClientObjectResolveContextBuilder();
            builder.Load(Mock.Of<IResolve<Field>>(), f => f.Id);
            builder.Load(Mock.Of<IResolve<Field>>(), f => f.Title);
            builder.Load(Mock.Of<IResolve<List>>(), l => l.Title);

            var ctx = builder.ToResolveContext();

            var fieldRetreivals = ctx
                .GetRetrievals<Field>()
                .Select(e => e.ExtractSinglePropertyAccess().Name)
                .ToArray();

            Assert.Equal(2, fieldRetreivals.Length);
            Assert.Contains("Id", fieldRetreivals);
            Assert.Contains("Title", fieldRetreivals);

            var listRetreivals = ctx.GetRetrievals<List>();
            var listTitle = Assert.Single(listRetreivals);
            Assert.Equal("Title", listTitle.ExtractSinglePropertyAccess().Name);

            Assert.Empty(ctx.GetRetrievals<ContentType>());
        }
    }
}
