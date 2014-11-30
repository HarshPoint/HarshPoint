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
                Web = ClientOM.Web,
                FieldId = new Guid("fa564e0f-0c70-4ab9-b863-0177e6ddd247"), // SPBuiltInFieldId.Title
            };

            prov.Provision();

            Assert.False(prov.FieldAdded);
            Assert.False(prov.FieldRemoved);
            Assert.False(prov.FieldUpdated);
        }

        [Fact]
        public void Field_id_is_generated_into_schema()
        {
            var fieldId = Guid.NewGuid();
            var prov = new HarshFieldSchemaXmlProvisioner()
            {
                Web = ClientOM.Web,
                FieldId = fieldId,
            };

        }
    }
}
