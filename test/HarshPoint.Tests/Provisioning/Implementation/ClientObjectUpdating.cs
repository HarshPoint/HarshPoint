using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning.Implementation
{
    public class ClientObjectUpdating : SharePointClientTest
    {
        public ClientObjectUpdating(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task Null_string_is_not_set()
        {
            var provisioner = new DescriptionProvisioner()
            {
                Description = null
            };

            var updater = new ClientObjectUpdater<DescriptionProvisioner, Field>(
                provisioner.Metadata
            );

            var description = "initial description";

            updater.Map(f => f.Description, p => p.Description);

            var field = await CreateField();
            field.Description = description;

            var changed = updater.Update(
                field,
                provisioner,
                provisioner.Metadata.Parameters
            );

            Assert.False(changed);
            Assert.Equal(description, field.Description);
        }

        [Fact]
        public async Task Equal_string_is_not_set()
        {
            var description = "initial description";

            var provisioner = new DescriptionProvisioner()
            {
                Description = description
            };

            var updater = new ClientObjectUpdater<DescriptionProvisioner, Field>(
                provisioner.Metadata
            );

            updater.Map(f => f.Description, p => p.Description);

            var field = await CreateField();
            field.Description = description;

            var changed = updater.Update(
                field,
                provisioner,
                provisioner.Metadata.Parameters
            );

            Assert.False(changed);
            Assert.Equal(description, field.Description);
        }

        [Fact]
        public async Task Different_string_is_set()
        {
            var provisioner = new DescriptionProvisioner()
            {
                Description = "42"
            };

            var updater = new ClientObjectUpdater<DescriptionProvisioner, Field>(
                provisioner.Metadata
            );

            updater.Map(f => f.Description, p => p.Description);

            var field = await CreateField();
            field.Description = "initial";

            var changed = updater.Update(
                field,
                provisioner,
                provisioner.Metadata.Parameters
            );

            Assert.True(changed);
            Assert.Equal("42", field.Description);
        }

        [Fact]
        public void Mapped_properties_are_returned_for_retrieval()
        {
            var provisioner = new DescriptionProvisioner();

            var updater = new ClientObjectUpdater<DescriptionProvisioner, Field>(
                provisioner.Metadata
            );

            updater.Map(f => f.Description, p => p.Description);

            var retrievals = updater.GetRetrievals();
            var descRetrieval = Assert.Single(retrievals);

            var property = descRetrieval.ExtractLastPropertyAccess();
            Assert.Equal("Description", property.Name);
            Assert.Equal(typeof(Field), property.DeclaringType);
        }

        private sealed class DescriptionProvisioner : HarshProvisioner
        {
            [Parameter]
            public String Description { get; set; }
        }
    }
}
