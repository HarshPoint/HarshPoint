using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using Moq;
using System;
using System.Xml.Linq;
using Xunit;

namespace HarshPoint.Tests.Provisioning
{
    public class HarshFieldSchemaXmlBuilderTests : IUseFixture<SharePointClientFixture>
    {
        public SharePointClientFixture ClientOM { get; set; }

        [Fact]
        public void Update_with_an_existing_field_calls_only_update_transforms()
        {
            var addOnlyTransformer = GetNopTransformer();
            var addOrUpdateTransformer = GetNopTransformer();

            addOnlyTransformer.Object.OnFieldAddOnly = true;
            addOrUpdateTransformer.Object.OnFieldAddOnly = false;

            var builder = new HarshFieldSchemaXmlBuilder()
            {
                Transformers = { addOnlyTransformer.Object, addOrUpdateTransformer.Object }
            };

            var titleField = GetTitleField();
            builder.Update(titleField, schemaXml: null);

            addOnlyTransformer.Verify(t => t.Transform(It.IsAny<XElement>()), Times.Never());
            addOrUpdateTransformer.Verify(t => t.Transform(It.IsAny<XElement>()), Times.Once());
        }

        [Fact]
        public void Update_a_new_field_calls_add_and_update_transforms()
        {
            var addOnlyTransformer = GetNopTransformer();
            var addOrUpdateTransformer = GetNopTransformer();

            addOnlyTransformer.Object.OnFieldAddOnly = true;
            addOrUpdateTransformer.Object.OnFieldAddOnly = false;

            var builder = new HarshFieldSchemaXmlBuilder()
            {
                Transformers = { addOnlyTransformer.Object, addOrUpdateTransformer.Object }
            };

            builder.Update(field: null, schemaXml: null);

            addOnlyTransformer.Verify(t => t.Transform(It.IsAny<XElement>()), Times.Once());
            addOrUpdateTransformer.Verify(t => t.Transform(It.IsAny<XElement>()), Times.Once());
        }

        [Fact]
        public void GetExistingSchemaXml_returns_empty_element_for_a_null_Field()
        {
            var expected = new XElement("Field");
            var actual = new HarshFieldSchemaXmlBuilder().GetExistingSchemaXml(null);
            Assert.Equal(expected, actual, new XNodeEqualityComparer());
        }

        [Fact]
        public void GetExistingSchemaXml_returns_schema_for_Title_Field()
        {
            var field = GetTitleField();
            var expected = XElement.Parse(field.SchemaXmlWithResourceTokens);
            var actual = new HarshFieldSchemaXmlBuilder().GetExistingSchemaXml(field);

            Assert.Equal(expected, actual, new XNodeEqualityComparer());
        }

        public void SetFixture(SharePointClientFixture data)
        {
            ClientOM = data;
        }

        private Field GetTitleField()
        {
            var field = ClientOM.Web.AvailableFields.GetByInternalNameOrTitle("Title");
            ClientOM.Context.Load(field, f => f.SchemaXmlWithResourceTokens);
            ClientOM.Context.ExecuteQuery();
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
