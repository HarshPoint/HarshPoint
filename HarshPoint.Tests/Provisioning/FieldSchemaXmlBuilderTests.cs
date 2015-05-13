using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using Moq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;

namespace HarshPoint.Tests.Provisioning
{
    public class HarshFieldSchemaXmlBuilderTests : IClassFixture<SharePointClientFixture>
    {
        public HarshFieldSchemaXmlBuilderTests(SharePointClientFixture data)
        {
            ClientOM = data;
        }

        public SharePointClientFixture ClientOM { get; set; }

        [Fact]
        public async Task Update_with_an_existing_field_calls_only_update_transforms()
        {
            var addOnlyTransformer = GetNopTransformer();
            var addOrUpdateTransformer = GetNopTransformer();

            addOnlyTransformer.Object.SkipWhenModifying = true;
            addOrUpdateTransformer.Object.SkipWhenModifying = false;

            var builder = new HarshFieldSchemaXmlBuilder()
            {
                Transformers = { addOnlyTransformer.Object, addOrUpdateTransformer.Object }
            };

            var titleField = await GetTitleField();
            await builder.Update(titleField, schemaXml: null);

            addOnlyTransformer.Verify(t => t.Transform(It.IsAny<XElement>()), Times.Never());
            addOrUpdateTransformer.Verify(t => t.Transform(It.IsAny<XElement>()), Times.Once());
        }

        [Fact]
        public async Task Update_a_new_field_calls_add_and_update_transforms()
        {
            var addOnlyTransformer = GetNopTransformer();
            var addOrUpdateTransformer = GetNopTransformer();

            addOnlyTransformer.Object.SkipWhenModifying = true;
            addOrUpdateTransformer.Object.SkipWhenModifying = false;

            var builder = new HarshFieldSchemaXmlBuilder()
            {
                Transformers = { addOnlyTransformer.Object, addOrUpdateTransformer.Object }
            };
            await builder.Update(field: null, schemaXml: null);

            addOnlyTransformer.Verify(t => t.Transform(It.IsAny<XElement>()), Times.Once());
            addOrUpdateTransformer.Verify(t => t.Transform(It.IsAny<XElement>()), Times.Once());
        }

        [Fact]
        public async Task GetExistingSchemaXml_returns_empty_element_for_a_null_Field()
        {
            var expected = new XElement("Field");
            var actual = await new HarshFieldSchemaXmlBuilder().GetExistingSchemaXml(null);
            Assert.Equal(expected, actual, new XNodeEqualityComparer());
        }

        [Fact]
        public async Task GetExistingSchemaXml_returns_schema_for_Title_Field()
        {
            var field = await GetTitleField();
            var expected = XElement.Parse(field.SchemaXmlWithResourceTokens);
            var actual = await new HarshFieldSchemaXmlBuilder().GetExistingSchemaXml(field);

            Assert.Equal(expected, actual, new XNodeEqualityComparer());
        }

        private async Task<Field> GetTitleField()
        {
            var field = ClientOM.Web.AvailableFields.GetByInternalNameOrTitle("Title");
            ClientOM.ClientContext.Load(field, f => f.SchemaXmlWithResourceTokens);
            await ClientOM.ClientContext.ExecuteQueryAsync();
            return field;
        }

        private Mock<HarshFieldSchemaXmlTransformer> GetNopTransformer()
        {
            var mock = new Mock<HarshFieldSchemaXmlTransformer>();
            
            mock
                .Setup(x => x.Transform(It.IsAny<XElement>()))
                .Returns<XElement>(xe => xe);

            return mock;
        }
    }
}
