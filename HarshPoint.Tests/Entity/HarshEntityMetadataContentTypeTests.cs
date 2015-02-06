using HarshPoint.Entity;
using HarshPoint.Entity.Metadata;
using System;
using System.Linq;
using Xunit;

namespace HarshPoint.Tests.Entity
{
    public class HarshEntityMetadataContentTypeTests
    {
        private const String DummyId = "0x0100a15a2839db44414a86489b400d47a319";
        
        [Fact]
        public void Fails_without_a_ContentTypeAttribute()
        {
            Assert.Throws(
                typeof(ArgumentOutOfRangeException),
                () => new HarshEntityMetadataContentType(typeof(NotAContentType))
            );
        }

        [Fact]
        public void Fails_when_not_inherited_form_HarshEntity()
        {
            Assert.Throws(
                typeof(ArgumentOutOfRangeException),
                () => new HarshEntityMetadataContentType(typeof(NotAnEntity))
            );
        }

        [Fact]
        public void Succeeds_with_a_ContentTypeAttribute()
        {
            var md = new HarshEntityMetadataContentType(typeof(EmptyContentType));
            Assert.Equal(DummyId, md.ContentTypeId, StringComparer.OrdinalIgnoreCase);
        }

        [Fact]
        public void Finds_readable_writable_properties_with_FieldAttribute()
        {
            var md = new HarshEntityMetadataContentType(typeof(ContentTypeWithFields));
            Assert.Single(md.DeclaredFields);

            var field = md.DeclaredFields.First();
            Assert.NotNull(field);
            Assert.Equal("FinallyAField", field.InternalName);
            Assert.Equal("FinallyAField", field.StaticName);
            Assert.Equal(new Guid("de5e71bc-8ba9-4693-84b4-615b97d4003e"), field.FieldId);
        }

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
        private sealed class ContentTypeWithFields : HarshEntity
        {
            [Field("68b4c509-ec16-4cd0-bb65-f57c54ddfc53")]
            public String WriteOnly
            {
                set { }
            }

            [Field("6bab39f8-8979-4f47-a5d5-56785eb45d4f")]
            public String ReadOnly
            {
                get { return null; }
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
    }
}
