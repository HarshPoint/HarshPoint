using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
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

        protected async Task<IEnumerable<T>> ResolveAsync<T>(IClientObjectResolveBuilder<T> builder)
            where T : ClientObject
        {
            var state = builder.Initialize(Fixture.ResolveContext);
            await ClientContext.ExecuteQueryAsync();
            var results = builder.ToEnumerable(state, Fixture.ResolveContext)
                .Cast<Object>()
                .ToArray();
            return results.Cast<T>();
        }
    }
}
