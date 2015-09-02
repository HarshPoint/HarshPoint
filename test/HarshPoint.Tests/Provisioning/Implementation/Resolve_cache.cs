using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System.Collections.Generic;
using System.Collections.Immutable;
using Xunit;
using Xunit.Abstractions;
using System.Linq;
using HarshPoint.Provisioning;
using System.Threading.Tasks;

namespace HarshPoint.Tests.Provisioning.Implementation
{
    public class Resolve_cache : SharePointClientTest
    {
        private readonly ResolveCache _cache;
        private readonly ClientObjectManualResolver _mr;

        public Resolve_cache(ITestOutputHelper output) : base(output)
        {
            _cache = new ResolveCache();

            _mr = new ClientObjectManualResolver(() =>
                new ClientObjectResolveContext(Context)
                {
                    Cache = _cache
                }
            );
        }

        [Fact]
        public void ClientObject_resolved_from_cache()
        {
            var rb = new TestObjectResolveBuilder();
            var result = _mr.ResolveSingle(rb);

            Assert.NotNull(result.Value);

            var result2 = _mr.ResolveSingle(rb);

            Assert.Same(result.Value, result2.Value);
        }

        [FactNeedsSharePoint]
        public async Task ClientObjectQuery_resolved_from_cache()
        {
            var rb = new TestQueryResolveBuilder().ByInternalName("Title");
            var result = _mr.ResolveSingle(rb);

            await ClientContext.ExecuteQueryAsync();

            Assert.NotNull(result.Value);

            var result2 = _mr.ResolveSingle(rb);

            Assert.False(ClientContext.HasPendingRequest);

            Assert.Same(result.Value, result2.Value);
        }

        [FactNeedsSharePoint]
        public async Task ClientObjectQuery_resolved_from_cache_with_additional_retrievals()
        {
            var rb = new TestQueryResolveBuilder().ByInternalName("Title");
            var result = _mr.ResolveSingle(rb);

            await ClientContext.ExecuteQueryAsync();

            Assert.NotNull(result.Value);
            Assert.False(result.Value.IsPropertyAvailable(f => f.FieldTypeKind));

            var result2 = _mr.ResolveSingle(rb, f => f.FieldTypeKind);

            Assert.True(ClientContext.HasPendingRequest);

            await ClientContext.ExecuteQueryAsync();

            Assert.Same(result.Value, result2.Value);
            Assert.True(result.Value.IsPropertyAvailable(f => f.FieldTypeKind));
        }

        private class TestObjectResolveBuilder :
            ClientObjectResolveBuilder<Field>
        {
            protected override IEnumerable<Field> CreateObjects(
                ClientObjectResolveContext context
            )
                => ImmutableArray.Create(
                    context
                    .ProvisionerContext
                    .Web
                    .Fields
                    .GetByInternalNameOrTitle("Title")
                );
        }

        private class TestQueryResolveBuilder :
             ClientObjectQueryResolveBuilder<Field>
        {
            protected override IQueryable<Field> CreateQuery(
                ClientObjectResolveContext context
            )
                => context.ProvisionerContext.Web.Fields.Include(f => f.Id);
        }
    }
}
