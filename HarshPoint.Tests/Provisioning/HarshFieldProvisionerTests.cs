using HarshPoint.Provisioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var prov = new HarshFieldProvisioner()
            {
                Web = ClientOM.Web,
                FieldId = new Guid("fa564e0f-0c70-4ab9-b863-0177e6ddd247"), // SPBuiltInFieldId.Title
            };

            prov.Provision();

            Assert.False(prov.FieldAdded);
            Assert.False(prov.FieldRemoved);
            Assert.False(prov.FieldUpdated);
        }
    }
}
