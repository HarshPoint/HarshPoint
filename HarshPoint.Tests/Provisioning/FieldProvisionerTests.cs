using HarshPoint.Provisioning;
using System;
using System.Threading.Tasks;
using Xunit;

namespace HarshPoint.Tests.Provisioning
{
    public class FieldProvisionerTests : IClassFixture<SharePointClientFixture>
    {
        public FieldProvisionerTests(SharePointClientFixture data)
        {
            ClientOM = data;
        }

        public SharePointClientFixture ClientOM
        {
            get;
            set;
        }

        [Fact]
        public async Task Existing_field_is_not_provisioned()
        {
            var prov = new HarshField()
            {
                Id = new Guid("fa564e0f-0c70-4ab9-b863-0177e6ddd247"), // SPBuiltInFieldId.Title
                DisplayName = "Title",
            };

            Assert.IsType(typeof(HarshProvisionerResult), await prov.ProvisionAsync(ClientOM.Context));
        }

        [Fact]
        public void Fails_if_id_missing()
        {
            var prov = new HarshField()
            {
                InternalName = "DummyField"
            };

            Assert.ThrowsAsync<InvalidOperationException>(async () => await prov.SchemaXmlBuilder.Update(null, null));
        }

        [Fact]
        public void Fails_if_InternalName_missing()
        {
            var prov = new HarshField()
            {
                Id = Guid.NewGuid(),
                DisplayName = "Dummy",
            };

            Assert.ThrowsAsync<InvalidOperationException>(async () => await prov.SchemaXmlBuilder.Update(null, null));
        }

        [Fact]
        public async Task Field_id_is_generated_into_schema()
        {
            var fieldId = Guid.NewGuid();
            var prov = new HarshField()
            {
                Id = fieldId,
                InternalName = "DummyField",
                DisplayName = "Dummy",
            };

            var schema = await prov.SchemaXmlBuilder.Update(null, null);

            Assert.NotNull(schema);
            Assert.Equal("Field", schema.Name);
            Assert.Equal(fieldId, new Guid(schema.Attribute("ID").Value));
        }

        [Fact]
        public async Task Field_DisplayName_is_generated_into_schema()
        {
            var prov = new HarshField()
            {
                Id = Guid.NewGuid(),
                InternalName = "DummyField",
                DisplayName = "Dummy",
            };

            var schema = await prov.SchemaXmlBuilder.Update(null, null);

            Assert.NotNull(schema);
            Assert.Equal("Field", schema.Name);
            Assert.Equal("Dummy", schema.Attribute("DisplayName").Value);
        }

        [Fact]
        public async Task Field_InternalName_is_generated_into_schema()
        {
            var prov = new HarshField()
            {
                Id = Guid.NewGuid(),
                InternalName = "DummyField",
                DisplayName = "Dummy",
            };

            var schema = await prov.SchemaXmlBuilder.Update(null, null);

            Assert.NotNull(schema);
            Assert.Equal("Field", schema.Name);
            Assert.Equal("DummyField", schema.Attribute("Name").Value);
        }

        [Fact]
        public async Task Field_no_StaticName_is_generated_into_schema()
        {
            var prov = new HarshField()
            {
                Id = Guid.NewGuid(),
                InternalName = "DummyField",
                DisplayName = "Dummy",
            };

            var schema = await prov.SchemaXmlBuilder.Update(null, null);

            Assert.NotNull(schema);
            Assert.Equal("Field", schema.Name);
            Assert.Equal("DummyField", schema.Attribute("StaticName").Value);
        }

        [Fact]
        public async Task Field_explicit_StaticName_is_generated_into_schema()
        {
            var fieldId = Guid.NewGuid();
            var prov = new HarshField()
            {
                Id = fieldId,
                InternalName = "DummyField",
                DisplayName = "Dummy",
                StaticName = "WhomDoYouCallDummy"
            };

            var schema = await prov.SchemaXmlBuilder.Update(null, null);

            Assert.NotNull(schema);
            Assert.Equal("WhomDoYouCallDummy", schema.Attribute("StaticName").Value);
        }

        [Fact]
        public async Task Field_group_is_generated_into_schema()
        {
            var fieldId = Guid.NewGuid();
            var prov = new HarshField()
            {
                Id = fieldId,
                InternalName = "DummyField",
                DisplayName = "Dummy",
                Group = "GROO GROO GROO"
            };

            var schema = await prov.SchemaXmlBuilder.Update(null, null);

            Assert.NotNull(schema);
            Assert.Equal("GROO GROO GROO", schema.Attribute("Group").Value);
        }

    }
}
