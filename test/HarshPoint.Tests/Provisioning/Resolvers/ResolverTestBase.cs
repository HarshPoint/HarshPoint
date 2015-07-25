using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning.Resolvers
{
    public class ResolverTestBase : SharePointClientTest
    {
        public ResolverTestBase(SharePointClientFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
        }

        protected async Task<IEnumerable<T>> ResolveAsync<T>(IResolveBuilder<T, ClientObjectResolveContext> builder)
            where T : ClientObject
        {
            var results = ManualResolver.Resolve(builder);

            await ClientContext.ExecuteQueryAsync();

            return results;
        }
    }
}
