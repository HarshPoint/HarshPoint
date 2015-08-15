using HarshPoint.Entity;
using HarshPoint.Entity.Metadata;
using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace HarshPoint.Tests.Entity
{
    public class HarshEntityMetadataContentTypeTests
    {
        private const String DummyId = "0x0100a15a2839db44414a86489b400d47a319";

        [Fact]
        public void Fails_without_a_ContentTypeAttribute()
        {
            Assert.Throws<ArgumentException>(
                () => CreateMetadata(typeof(NotAContentType))
            );
        }

        [Fact]
        public void Fails_when_not_inherited_form_HarshEntity()
        {
            Assert.Throws<ArgumentException>(
                () => CreateMetadata(typeof(NotAnEntity))
            );
        }

        [Fact]
        public void Succeeds_with_a_ContentTypeAttribute()
        {
            var md = CreateMetadata(typeof(EmptyContentType));
            Assert.Equal(DummyId, md.ContentTypeId, StringComparer.OrdinalIgnoreCase);
        }

        [Fact]
        public void Finds_readable_writable_properties_with_FieldAttribute()
        {
            var md = CreateMetadata(typeof(ContentTypeWithFields));
            Assert.Single(md.DeclaredFields);

            var field = md.DeclaredFields.First();
            Assert.NotNull(field);
            Assert.Equal("FinallyAField", field.InternalName);
            Assert.Equal("FinallyAField", field.StaticName);
            Assert.Equal(new Guid("de5e71bc-8ba9-4693-84b4-615b97d4003e"), field.FieldId);
        }

        [Fact]
        public void Has_correct_id_with_a_base_type()
        {
            var md = CreateMetadata(typeof(ContentTypeWithBaseType));
            Assert.Equal(DummyId + "01", md.ContentTypeId, StringComparer.OrdinalIgnoreCase);
        }

        [Fact]
        public void Has_declared_fields_with_a_base_type()
        {
            var md = CreateMetadata(typeof(ContentTypeWithBaseType));
            Assert.Single(md.DeclaredFields);

            var field = md.DeclaredFields.First();
            Assert.NotNull(field);
            Assert.Equal("AnotherField", field.InternalName);
            Assert.Equal("AnotherField", field.StaticName);
            Assert.Equal(new Guid("de5e71bc-8ba9-4693-84b4-615b97d4003f"), field.FieldId);
        }

        private static HarshEntityMetadataContentType CreateMetadata(Type type)
            => new HarshEntityMetadataContentType(
                HarshEntityMetadataRepository.Current,
                type.GetTypeInfo()
            );

        private sealed class NotAContentType
        {
        }

        [ContentType(DummyId)]
        private sealed class NotAnEntity
        {
        }

        [ContentType(DummyId)]
        private sealed class EmptyContentType : HarshEntity
        {
        }

        [ContentType(DummyId)]
        private class ContentTypeWithFields : HarshEntity
        {
            [Field("68b4c509-ec16-4cd0-bb65-f57c54ddfc53")]
            public String WriteOnly
            {
                set { }
            }

            [Field("68b4c510-ec16-4cd0-bb65-f57c54ddfc53")]
            public String WritePrivate
            {
                get;

            }

            [Field("6bab39f8-8979-4f47-a5d5-56785eb45d4f")]
            public String ReadOnly => null;

            [Field("6bab39f9-8979-4f47-a5d5-56785eb45d4f")]
            public String ReadPrivate
            {
                private get;
                set;
            }

            [Field("6bab3a00-8979-4f47-a5d5-56785eb45d4f")]
            internal String Internal
            {
                get;
                set;
            }

            public String NotAField
            {
                get;
                set;
            }

            [Field("de5e71bc-8ba9-4693-84b4-615b97d4003e")]
            public String FinallyAField
            {
                get;
                set;
            }
        }

        [ContentType("01")]
        private class ContentTypeWithBaseType : ContentTypeWithFields
        {
            [Field("de5e71bc-8ba9-4693-84b4-615b97d4003f")]
            public String AnotherField
            {
                get;
                set;
            }
        }
    }
}
