using HarshPoint.ObjectModel;
using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning
{
    public class DefaultFromContextTests : SharePointClientTest
    {
        public DefaultFromContextTests(SharePointClientFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
        }

        [Fact]
        public void Metadata_finds_DefaultFromContext_single_resolver()
        {
            var metadata = new HarshProvisionerMetadata(typeof(SingleResolverProvisioner));

            var param = Assert.Single(
                metadata.DefaultFromContextPropertyBinder.Properties
            );

            Assert.Equal(
                typeof(SingleResolverProvisioner).GetProperty("SingleResolver"),
                param.PropertyInfo
            );

            Assert.Equal(
                typeof(String),
                param.ResolvedPropertyInfo?.ResolvedType
            );
        }

        [Fact]
        public void Metadata_finds_DefaultFromContext_resolver()
        {
            var metadata = new HarshProvisionerMetadata(typeof(ResolverProvisioner));

            var param = Assert.Single(
                metadata.DefaultFromContextPropertyBinder.Properties
            );

            Assert.Equal(
                typeof(ResolverProvisioner).GetProperty("Resolver"),
                param.PropertyInfo
            );

            Assert.Equal(
                typeof(String),
                param.ResolvedPropertyInfo?.ResolvedType
            );
        }

        [Fact]
        public void Resolve_property_gets_assigned()
        {
            var prov = new ResolverProvisioner();

            Assert.Null(prov.Resolver);

            prov.Metadata.DefaultFromContextPropertyBinder.Bind(
                prov,
                Fixture.Context
            );

            Assert.NotNull(prov.Resolver);
            Assert.IsType<ContextStateResolver<String>>(prov.Resolver);
        }

        [Fact]
        public void ResolveSingle_property_gets_assigned()
        {
            var prov = new SingleResolverProvisioner();

            Assert.Null(prov.SingleResolver);

            prov.Metadata.DefaultFromContextPropertyBinder.Bind(
                prov,
                Fixture.Context
            );

            Assert.NotNull(prov.SingleResolver);
            Assert.IsType<ContextStateResolver<String>>(prov.SingleResolver);
        }

        [Fact]
        public void String_property_gets_assigned()
        {
            var prov = new StringProvisioner();

            Assert.Null(prov.StringProperty);

            prov.Metadata.DefaultFromContextPropertyBinder.Bind(
                prov,
                Fixture.Context.PushState("42")
            );

            Assert.Equal("42", prov.StringProperty);
        }

        [Fact]
        public void Tagged_property_gets_assigned()
        {
            var prov = new TaggedProvisioner();
            Assert.Null(prov.TaggedStringProperty);

            var state = Fixture.Context
                .PushState("red herring")
                .PushState(new DummyTag() { Value = "424242" });

            prov.Metadata.DefaultFromContextPropertyBinder.Bind(
                prov,
                state
            );

            Assert.Equal("424242", prov.TaggedStringProperty);
        }

        [Fact]
        public void Assigned_property_doesnt_get_overwritten()
        {
            var prov = new ResolverProvisioner();
            var resolver = Mock.Of<IResolve<String>>();
            prov.Resolver = resolver;

            prov.Metadata.DefaultFromContextPropertyBinder.Bind(
                prov,
                Fixture.Context
            );

            Assert.Same(resolver, prov.Resolver);
        }

        [Fact]
        public void Fails_if_TagType_doesnt_implement_IDefaultFromContextTag()
        {
            Assert.Throws<HarshObjectMetadataException>(
                () => new HarshProvisionerMetadata(typeof(WrongTagType))
            );
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

        private sealed class WrongTagType : HarshProvisioner
        {
            [Parameter]
            [DefaultFromContext(typeof(String))]
            public String Param { get; set; }
        }
    }
}
