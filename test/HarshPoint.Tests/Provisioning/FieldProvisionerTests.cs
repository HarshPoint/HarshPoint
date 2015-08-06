using HarshPoint.ObjectModel;
using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning
{

    public class When_provisioning_existing_field : SharePointClientTest
    {
        public When_provisioning_existing_field(SharePointClientFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
        }

        [Fact]
        public async Task Field_is_not_created()
        {
            var prov = new HarshField()
            {
                Id = HarshBuiltInFieldId.Title,
            };

            await prov.ProvisionAsync(Context);
            var fo = FindOutput<Field>();

            Assert.False(fo.ObjectCreated);
        }
    }


    public class Provisioning_field_fails : SharePointClientTest
    {
        public Provisioning_field_fails(SharePointClientFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
        }

        [Fact]
        public Task If_Id_empty()
        {
            var prov = new HarshField()
            {
                InternalName = Guid.NewGuid().ToString("n")
            };

            return Assert.ThrowsAsync<ParameterValidationException>(
                () => prov.ProvisionAsync(Context)
            );
        }

        [Fact]
        public Task If_InternalName_null()
        {
            var prov = new HarshField()
            {
                Id = Guid.NewGuid(),
            };

            return Assert.ThrowsAsync<ParameterValidationException>(
                () => prov.ProvisionAsync(Context)
            );
        }
    }

    public class When_provisioning_new_field : SharePointClientTest
    {
        public When_provisioning_new_field(SharePointClientFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
        }

        [Fact]
        public async Task Field_is_created()
        {
            var fieldId = Guid.NewGuid();
            var fieldName = fieldId.ToString("n");

            var provisioner = new HarshField()
            {
                Id = fieldId,
                InternalName = fieldName,
            };

            await provisioner.ProvisionAsync(Context);

            var fieldOutput = FindOutput<Field>();
            Assert.True(fieldOutput.ObjectCreated);

            var field = fieldOutput.Object;
            Assert.NotNull(field);
        }

        [Fact]
        public async Task Id_is_set()
        {
            var fieldId = Guid.NewGuid();
            var fieldName = fieldId.ToString("n");

            var provisioner = new HarshField()
            {
                Id = fieldId,
                InternalName = fieldName,
            };

            await provisioner.ProvisionAsync(Context);

            var fieldOutput = FindOutput<Field>();
            var field = fieldOutput.Object;

            ClientContext.Load(field, f => f.Id);
            await Context.ClientContext.ExecuteQueryAsync();

            Assert.Equal(fieldId, field.Id);
        }

        [Fact]
        public async Task InternalName_is_set()
        {
            var fieldId = Guid.NewGuid();
            var fieldName = fieldId.ToString("n");

            var provisioner = new HarshField()
            {
                Id = fieldId,
                InternalName = fieldName,
            };

            await provisioner.ProvisionAsync(Context);

            var fieldOutput = FindOutput<Field>();
            var field = fieldOutput.Object;

            ClientContext.Load(field, f => f.InternalName);
            await Context.ClientContext.ExecuteQueryAsync();

            Assert.Equal(fieldName, field.InternalName);
        }

        [Fact]
        public async Task Field_implicit_StaticName_is_set()
        {
            var fieldId = Guid.NewGuid();
            var fieldName = fieldId.ToString("n");

            var provisioner = new HarshField()
            {
                Id = fieldId,
                InternalName = fieldName,
            };

            await provisioner.ProvisionAsync(Context);

            var fieldOutput = FindOutput<Field>();
            var field = fieldOutput.Object;

            ClientContext.Load(field, f => f.StaticName);
            await Context.ClientContext.ExecuteQueryAsync();

            Assert.Equal(fieldName, field.StaticName);
        }

        [Fact]
        public async Task Field_explicit_StaticName_is_set()
        {

            var fieldId = Guid.NewGuid();
            var fieldName = fieldId.ToString("n");
            var fieldNameStatic = $"{fieldName}Static";

            var provisioner = new HarshField()
            {
                Id = fieldId,
                InternalName = fieldName,
                StaticName = fieldNameStatic,
            };

            await provisioner.ProvisionAsync(Context);

            var fieldOutput = FindOutput<Field>();
            var field = fieldOutput.Object;

            ClientContext.Load(field, f => f.StaticName);
            await Context.ClientContext.ExecuteQueryAsync();

            Assert.Equal(fieldNameStatic, field.StaticName);
        }
    }
}
