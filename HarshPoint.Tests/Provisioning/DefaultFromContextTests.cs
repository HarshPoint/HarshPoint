using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HarshPoint.Tests.Provisioning
{
    public class DefaultFromContextTests : IClassFixture<SharePointClientFixture>
    {
        public DefaultFromContextTests(SharePointClientFixture fixture)
        {
            ClientOM = fixture;
        }

        public SharePointClientFixture ClientOM { get; private set; }

        [Fact]
        public void Metadata_throws_on_DefaultFromContext_on_a_non_resolver_property()
        {
            Assert.Throws<InvalidOperationException>(
                () => new HarshProvisionerMetadata(typeof(InvalidProvisioner))
            );
        }

        [Fact]
        public void Metadata_finds_DefaultFromContext_single_resolver()
        {
            var metadata = new HarshProvisionerMetadata(typeof(SingleResolverProvisioner));

            var prop = Assert.Single(
                metadata.DefaultFromContextProperties
            );

            Assert.Equal(
                typeof(SingleResolverProvisioner).GetProperty("SingleResolver"),
                prop.Property
            );

            Assert.Equal(
                typeof(String),
                prop.ResolvedType
            );
        }

        [Fact]
        public void Metadata_finds_DefaultFromContext_resolver()
        {
            var metadata = new HarshProvisionerMetadata(typeof(ResolverProvisioner));

            var prop = Assert.Single(
                metadata.DefaultFromContextProperties
            );

            Assert.Equal(
                typeof(ResolverProvisioner).GetProperty("Resolver"),
                prop.Property
            );

            Assert.Equal(
                typeof(String),
                prop.ResolvedType
            );
        }

        [Fact]
        public async Task DefaultFromContext_property_gets_assigned()
        {
            var prov = new ResolverProvisioner();

            Assert.Null(prov.Resolver);

            await prov.ProvisionAsync(ClientOM.Context);

            Assert.NotNull(prov.Resolver);
            Assert.IsType<ContextStateResolver<String>>(prov.Resolver);
        }

        [Fact]
        public async Task DefaultFromContext_assigned_property_doesnt_get_overwritten()
        {
            var prov = new ResolverProvisioner();
            var resolver = new DummyResolver();
            prov.Resolver = resolver;

            await prov.ProvisionAsync(ClientOM.Context);

            Assert.Same(resolver, prov.Resolver);
        }

        private class InvalidProvisioner : HarshProvisioner
        {
            [DefaultFromContext]
            public String InvalidDefaultFromContext
            {
                get;
                set;
            }
        }

        private class SingleResolverProvisioner : HarshProvisioner
        {
            [DefaultFromContext]
            public IResolve<String> SingleResolver
            {
                get;
                set;
            }
        }

        private class ResolverProvisioner : HarshProvisioner
        {
            [DefaultFromContext]
            public IResolve<String> Resolver
            {
                get;
                set;
            }
        }

        private class DummyResolver : IResolve<String>
        {
            public Task<IEnumerable<string>> ResolveAsync(HarshProvisionerContextBase context)
            {
                throw new NotImplementedException();
            }
        }
    }
}
