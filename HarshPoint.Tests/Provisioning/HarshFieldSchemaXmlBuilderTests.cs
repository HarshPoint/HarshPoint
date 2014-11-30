using HarshPoint.Provisioning;
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
            throw new NotImplementedException();
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
            var field = ClientOM.Web.AvailableFields.GetByInternalNameOrTitle("Title");
            ClientOM.Context.Load(field, f => f.SchemaXmlWithResourceTokens);
            ClientOM.Context.ExecuteQuery();
            var expected = XElement.Parse(field.SchemaXmlWithResourceTokens);
            var actual = new HarshFieldSchemaXmlBuilder().GetExistingSchemaXml(field);

            Assert.Equal(expected, actual, new XNodeEqualityComparer());
        }

        public void SetFixture(SharePointClientFixture data)
        {
            ClientOM = data;
        }
    }
}
