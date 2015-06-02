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

            Assert.Single(
                metadata.DefaultFromContextProperties, 
                typeof(SingleResolverProvisioner).GetProperty("SingleResolver")
            );
        }


        [Fact]
        public void Metadata_finds_DefaultFromContext_resolver()
        {
            var metadata = new HarshProvisionerMetadata(typeof(ResolverProvisioner));

            Assert.Single(
                metadata.DefaultFromContextProperties,
                typeof(ResolverProvisioner).GetProperty("Resolver")
            );
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
    }
}
