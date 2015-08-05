using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning
{
    public class FieldProvisionerTests : SharePointClientTest
    {
        public FieldProvisionerTests(SharePointClientFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
        }

        [Fact]
        public async Task Existing_field_is_not_provisioned()
        {
            var ctx = Fixture.CreateContext();

            var prov = new HarshField()
            {
                Id = HarshBuiltInFieldId.Title,
                DisplayName = "Title",
            };

            await prov.ProvisionAsync(ctx);
            var fo = FindOutput<Field>();

            Assert.False(fo.ObjectCreated);
        }

        [Fact]
        public async Task Existing_field_DisplayName_gets_changed()
        {
            var ctx = Fixture.CreateContext();

            var id = Guid.NewGuid();
            var internalName = id.ToString("n");
            try
            {
                var prov = new HarshField()
                {
                    Id = id,
                    InternalName = internalName,
                    DisplayName = internalName + "-before"
                };

                await prov.ProvisionAsync(ctx);

                var fo = FindOutput<Field>();

                Assert.True(fo.ObjectCreated);

                ctx.ClientContext.Load(fo.Object, f => f.Title);
                await ctx.ClientContext.ExecuteQueryAsync();

                Assert.Equal(prov.DisplayName, fo.Object.Title);

                prov = new HarshField()
                {
                    Id = id,
                    InternalName = internalName,
                    DisplayName = internalName + "-after"
                };

                await prov.ProvisionAsync(ctx);

                fo = FindOutput<Field>();
                Assert.False(fo.ObjectCreated);

                ctx.ClientContext.Load(fo.Object, f => f.Title);
                await ctx.ClientContext.ExecuteQueryAsync();

                Assert.Equal(prov.DisplayName, fo.Object.Title);
            }
            finally
            {
                Fixture.Web.Fields.GetById(id).DeleteObject();

                try
                {
                    await Fixture.ClientContext.ExecuteQueryAsync();
                }
                catch (ServerException)
                {
                }
            }

        }

        [Fact]
        public void Fails_if_id_missing()
        {
            var prov = new HarshField()
            {
                InternalName = "DummyField"
            };

            Assert.Throws<InvalidOperationException>(() => prov.SchemaXmlBuilder.Create());
        }

        [Fact]
        public void Fails_if_InternalName_missing()
        {
            var prov = new HarshField()
            {
                Id = Guid.NewGuid(),
                DisplayName = "Dummy",
            };

            Assert.Throws<InvalidOperationException>(() => prov.SchemaXmlBuilder.Create());
        }

        [Fact]
        public void Field_id_is_generated_into_schema()
        {
            var fieldId = Guid.NewGuid();
            var prov = new HarshField()
            {
                Id = fieldId,
                InternalName = "DummyField",
                DisplayName = "Dummy",
            };

            var schema = prov.SchemaXmlBuilder.Create();

            Assert.NotNull(schema);
            Assert.Equal("Field", schema.Name);
            Assert.Equal(fieldId, new Guid(schema.Attribute("ID").Value));
        }

        [Fact]
        public void Field_DisplayName_is_generated_into_schema()
        {
            var prov = new HarshField()
            {
                Id = Guid.NewGuid(),
                InternalName = "DummyField",
                DisplayName = "Dummy",
            };

            var schema = prov.SchemaXmlBuilder.Create();

            Assert.NotNull(schema);
            Assert.Equal("Field", schema.Name);
            Assert.Equal("Dummy", schema.Attribute("DisplayName").Value);
        }

        [Fact]
        public void Field_InternalName_is_generated_into_schema()
        {
            var prov = new HarshField()
            {
                Id = Guid.NewGuid(),
                InternalName = "DummyField",
                DisplayName = "Dummy",
            };

            var schema = prov.SchemaXmlBuilder.Create();

            Assert.NotNull(schema);
            Assert.Equal("Field", schema.Name);
            Assert.Equal("DummyField", schema.Attribute("Name").Value);
        }

        [Fact]
        public void Field_implicit_StaticName_is_generated_into_schema()
        {
            var prov = new HarshField()
            {
                Id = Guid.NewGuid(),
                InternalName = "DummyField",
                DisplayName = "Dummy",
            };

            var schema = prov.SchemaXmlBuilder.Create();

            Assert.NotNull(schema);
            Assert.Equal("Field", schema.Name);
            Assert.Equal("DummyField", schema.Attribute("StaticName").Value);
        }

        [Fact]
        public void Field_explicit_StaticName_is_generated_into_schema()
        {
            var fieldId = Guid.NewGuid();
            var prov = new HarshField()
            {
                Id = fieldId,
                InternalName = "DummyField",
                DisplayName = "Dummy",
                StaticName = "WhomDoYouCallDummy"
            };

            var schema = prov.SchemaXmlBuilder.Create();

            Assert.NotNull(schema);
            Assert.Equal("WhomDoYouCallDummy", schema.Attribute("StaticName").Value);
        }

        [Fact]
        public void Field_group_is_generated_into_schema()
        {
            var fieldId = Guid.NewGuid();
            var prov = new HarshField()
            {
                Id = fieldId,
                InternalName = "DummyField",
                DisplayName = "Dummy",
                Group = "GROO GROO GROO"
            };

            var schema = prov.SchemaXmlBuilder.Create();

            Assert.NotNull(schema);
            Assert.Equal("GROO GROO GROO", schema.Attribute("Group").Value);
        }

    }
}
