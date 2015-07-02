using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public void Metadata_finds_DefaultFromContext_single_resolver()
        {
            var metadata = new HarshProvisionerMetadata(typeof(SingleResolverProvisioner));

            var param = Assert.Single(
                metadata.DefaultParameterSet.Parameters
                    .Where(p => p.DefaultFromContext != null)
            );

            Assert.Equal(
                typeof(SingleResolverProvisioner).GetProperty("SingleResolver"),
                param.PropertyInfo
            );

            Assert.Equal(
                typeof(String),
                param.DefaultFromContext.ResolvedType
            );
        }

        [Fact]
        public void Metadata_finds_DefaultFromContext_resolver()
        {
            var metadata = new HarshProvisionerMetadata(typeof(ResolverProvisioner));

            var param = Assert.Single(
                metadata.DefaultParameterSet.Parameters
                    .Where(p => p.DefaultFromContext != null)
            );

            Assert.Equal(
                typeof(ResolverProvisioner).GetProperty("Resolver"),
                param.PropertyInfo
            );

            Assert.Equal(
                typeof(String),
                param.DefaultFromContext.ResolvedType
            );
        }

        [Fact]
        public async Task Resolve_property_gets_assigned()
        {
            var prov = new ResolverProvisioner();

            Assert.Null(prov.Resolver);

            await prov.ProvisionAsync(ClientOM.Context);

            Assert.NotNull(prov.Resolver);
            Assert.IsType<ContextStateResolver<String>>(prov.Resolver);
        }

        [Fact]
        public async Task ResolveSingle_property_gets_assigned()
        {
            var prov = new SingleResolverProvisioner();

            Assert.Null(prov.SingleResolver);

            await prov.ProvisionAsync(ClientOM.Context);

            Assert.NotNull(prov.SingleResolver);
            Assert.IsType<ContextStateResolver<String>>(prov.SingleResolver);
        }

        [Fact]
        public async Task String_property_gets_assigned()
        {
            var prov = new StringProvisioner();

            Assert.Null(prov.StringProperty);

            await prov.ProvisionAsync(ClientOM.Context.PushState("42"));

            Assert.Equal("42", prov.StringProperty);
        }

        [Fact]
        public async Task Tagged_property_gets_assigned()
        {
            var prov = new TaggedProvisioner();
            Assert.Null(prov.TaggedStringProperty);

            var state = ClientOM.Context
                .PushState("red herring")
                .PushState(new DummyTag() { Value = "424242" });

            await prov.ProvisionAsync(state);

            Assert.Equal("424242", prov.TaggedStringProperty);
        }

        [Fact]
        public async Task Assigned_property_doesnt_get_overwritten()
        {
            var prov = new ResolverProvisioner();
            var resolver = new DummyResolver();
            prov.Resolver = resolver;

            await prov.ProvisionAsync(ClientOM.Context);

            Assert.Same(resolver, prov.Resolver);
        }

        private class StringProvisioner : HarshProvisioner
        {
            [Parameter]
            [DefaultFromContext]
            public String StringProperty
            {
                get;
                set;
            }
        }

        private class TaggedProvisioner : HarshProvisioner
        {
            [Parameter]
            [DefaultFromContext(typeof(DummyTag))]
            public String TaggedStringProperty
            {
                get;
                set;
            }
        }

        private class SingleResolverProvisioner : HarshProvisioner
        {
            [Parameter]
            [DefaultFromContext]
            public IResolve<String> SingleResolver
            {
                get;
                set;
            }
        }

        private class ResolverProvisioner : HarshProvisioner
        {
            [Parameter]
            [DefaultFromContext]
            public IResolve<String> Resolver
            {
                get;
                set;
            }
        }

        private class DummyTag : DefaultFromContextTag<String>
        {
        }

        private class DummyResolver : IResolve<String>
        {
            public Task<IEnumerable<string>> TryResolveAsync(IResolveContext context)
            {
                throw new NotImplementedException();
            }
        }
    }
}
