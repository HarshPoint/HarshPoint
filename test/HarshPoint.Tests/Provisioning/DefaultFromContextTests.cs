﻿using HarshPoint.ObjectModel;
using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using Moq;
using System;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning
{
    public class DefaultFromContextTests : SharePointClientTest
    {
        public DefaultFromContextTests(ITestOutputHelper output)
            : base(output)
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
                Context
            );

            Assert.NotNull(prov.Resolver);
            Assert.IsType<ContextStateResolveBuilder<String>>(prov.Resolver);
        }

        [Fact]
        public void ResolveSingle_property_gets_assigned()
        {
            var prov = new SingleResolverProvisioner();

            Assert.Null(prov.SingleResolver);

            prov.Metadata.DefaultFromContextPropertyBinder.Bind(
                prov,
                Context
            );

            Assert.NotNull(prov.SingleResolver);
            Assert.IsType<ContextStateResolveBuilder<String>>(prov.SingleResolver);
        }

        [Fact]
        public void String_property_gets_assigned()
        {
            var prov = new StringProvisioner();

            Assert.Null(prov.StringProperty);

            prov.Metadata.DefaultFromContextPropertyBinder.Bind(
                prov,
                Context.PushState("42")
            );

            Assert.Equal("42", prov.StringProperty);
        }

        [Fact]
        public void Tagged_property_gets_assigned()
        {
            var prov = new TaggedProvisioner();
            Assert.Null(prov.TaggedStringProperty);

            var state = Context
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
                Context
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

        [Fact]
        public void ValueSource_is_retrievable()
        {
            var prov = new TaggedProvisioner();
            Assert.Null(prov.TaggedStringProperty);

            var ctx = Context.PushState(new DummyTag() { Value = "424242" });

            prov.Metadata.DefaultFromContextPropertyBinder.Bind(prov, ctx);

            Assert.True(prov.IsValueDefaultFromContext(() => prov.TaggedStringProperty));
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
