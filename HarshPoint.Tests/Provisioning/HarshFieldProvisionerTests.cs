using HarshPoint.Provisioning;
using System;
using System.Xml.Linq;
using Xunit;

namespace HarshPoint.Tests.Provisioning
{
    public class HarshFieldProvisionerTests : IUseFixture<SharePointClientFixture>
    {
        public SharePointClientFixture ClientOM
        {
            get;
            set;
        }

        public void SetFixture(SharePointClientFixture data)
        {
            ClientOM = data;
        }

        [Fact]
        public void Existing_field_is_not_provisioned()
        {
            var prov = new HarshFieldSchemaXmlProvisioner()
            {
                FieldId = new Guid("fa564e0f-0c70-4ab9-b863-0177e6ddd247"), // SPBuiltInFieldId.Title
            };

            prov.Provision(ClientOM.Context);

            Assert.False(prov.FieldAdded);
            Assert.False(prov.FieldRemoved);
            Assert.False(prov.FieldUpdated);
        }

        [Fact]
        public void Fails_if_id_missing()
        {
            var prov = new HarshFieldSchemaXmlProvisioner()
            {
                InternalName = "DummyField"
            };

            Assert.Throws<InvalidOperationException>(() => prov.SchemaXmlBuilder.Update(null, null));
        }

        [Fact]
        public void Fails_if_InternalName_missing()
        {
            var prov = new HarshFieldSchemaXmlProvisioner()
            {
                FieldId = Guid.NewGuid()
            };

            Assert.Throws<InvalidOperationException>(() => prov.SchemaXmlBuilder.Update(null, null));
        }

        [Fact]
        public void Field_id_is_generated_into_schema()
        {
            var fieldId = Guid.NewGuid();
            var prov = new HarshFieldSchemaXmlProvisioner()
            {
                FieldId = fieldId,
                InternalName = "DummyField"
            };

            var schema = prov.SchemaXmlBuilder.Update(null, null);

            Assert.NotNull(schema);
            Assert.Equal("Field", schema.Name);
            Assert.Equal(fieldId, new Guid(schema.Attribute("ID").Value));
        }

        [Fact]
        public void Field_InternalName_is_generated_into_schema()
        {
            var prov = new HarshFieldSchemaXmlProvisioner()
            {
                FieldId = Guid.NewGuid(),
                InternalName = "DummyField"
            };

            var schema = prov.SchemaXmlBuilder.Update(null, null);

            Assert.NotNull(schema);
            Assert.Equal("Field", schema.Name);
            Assert.Equal("DummyField", schema.Attribute("InternalName").Value);
        }

        [Fact]
        public void Field_no_StaticName_is_generated_into_schema()
        {
            var prov = new HarshFieldSchemaXmlProvisioner()
            {
                FieldId = Guid.NewGuid(),
                InternalName = "DummyField"
            };

            var schema = prov.SchemaXmlBuilder.Update(null, null);

            Assert.NotNull(schema);
            Assert.Equal("Field", schema.Name);
            Assert.Null(schema.Attribute("StaticName"));
        }

        [Fact]
        public void Field_explicit_StaticName_is_generated_into_schema()
        {
            var fieldId = Guid.NewGuid();
            var prov = new HarshFieldSchemaXmlProvisioner()
            {
                FieldId = fieldId,
                InternalName = "DummyField",
                StaticName = "WhomDoYouCallDummy"
            };

            var schema = prov.SchemaXmlBuilder.Update(null, null);

            Assert.NotNull(schema);
            Assert.Equal("Field", schema.Name);
            Assert.Equal("DummyField", schema.Attribute("InternalName").Value);
            Assert.Equal("WhomDoYouCallDummy", schema.Attribute("StaticName").Value);
        }
    }
}
