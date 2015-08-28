using HarshPoint.ObjectModel;
using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning
{

    public class When_provisioning_existing_field : SharePointClientTest
    {
        public When_provisioning_existing_field(ITestOutputHelper output)
            : base(output)
        {
        }

        [FactNeedsSharePoint]
        public async Task Field_is_not_created()
        {
            var prov = new HarshField()
            {
                Id = HarshBuiltInFieldId.Title,
            };

            await prov.ProvisionAsync(Context);
            var fo = LastObjectOutput<Field>();

            Assert.False(fo.ObjectAdded);
        }
    }


    public class Provisioning_field_fails : SharePointClientTest
    {
        public Provisioning_field_fails(ITestOutputHelper output)
            : base(output)
        {
        }

        [FactNeedsSharePoint]
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

        [FactNeedsSharePoint]
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
        public When_provisioning_new_field(ITestOutputHelper output)
            : base(output)
        {
        }

        [FactNeedsSharePoint]
        public async Task Field_is_created()
        {
            var fieldId = Guid.NewGuid();
            var fieldName = fieldId.ToString("n");

            var provisioner = new HarshField()
            {
                Id = fieldId,
                InternalName = fieldName,
                Type = FieldType.Text,
            };

            await provisioner.ProvisionAsync(Context);

            var fieldOutput = LastObjectOutput<Field>();
            Assert.True(fieldOutput.ObjectAdded);

            var field = fieldOutput.Object;
            Assert.NotNull(field);
        }

        [FactNeedsSharePoint]
        public async Task Id_is_set()
        {
            var fieldId = Guid.NewGuid();
            var fieldName = fieldId.ToString("n");

            var provisioner = new HarshField()
            {
                Id = fieldId,
                InternalName = fieldName,
                Type = FieldType.Text,
            };

            await provisioner.ProvisionAsync(Context);

            var fieldOutput = LastObjectOutput<Field>();
            var field = fieldOutput.Object;

            ClientContext.Load(field, f => f.Id);
            await Context.ClientContext.ExecuteQueryAsync();

            Assert.Equal(fieldId, field.Id);
        }

        [FactNeedsSharePoint]
        public async Task InternalName_is_set()
        {
            var fieldId = Guid.NewGuid();
            var fieldName = fieldId.ToString("n");

            var provisioner = new HarshField()
            {
                Id = fieldId,
                InternalName = fieldName,
                Type = FieldType.Text,
            };

            await provisioner.ProvisionAsync(Context);

            var fieldOutput = LastObjectOutput<Field>();
            var field = fieldOutput.Object;

            ClientContext.Load(field, f => f.InternalName);
            await Context.ClientContext.ExecuteQueryAsync();

            Assert.Equal(fieldName, field.InternalName);
        }

        [FactNeedsSharePoint]
        public async Task Field_implicit_StaticName_is_set()
        {
            var fieldId = Guid.NewGuid();
            var fieldName = fieldId.ToString("n");

            var provisioner = new HarshField()
            {
                Id = fieldId,
                InternalName = fieldName,
                Type = FieldType.Text,
            };

            await provisioner.ProvisionAsync(Context);

            var fieldOutput = LastObjectOutput<Field>();
            var field = fieldOutput.Object;

            ClientContext.Load(field, f => f.StaticName);
            await Context.ClientContext.ExecuteQueryAsync();

            Assert.Equal(fieldName, field.StaticName);
        }

        [FactNeedsSharePoint]
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
                Type = FieldType.Text,
            };

            await provisioner.ProvisionAsync(Context);

            var fieldOutput = LastObjectOutput<Field>();
            var field = fieldOutput.Object;

            ClientContext.Load(field, f => f.StaticName);
            await Context.ClientContext.ExecuteQueryAsync();

            Assert.Equal(fieldNameStatic, field.StaticName);
        }
    }

    public class When_unprovisioning : SharePointClientTest
    {
        public When_unprovisioning(ITestOutputHelper output) : base(output)
        {
        }

        [FactNeedsSharePoint]
        public async Task Nonexistent_field_is_not_removed()
        {
            var prov = new HarshField()
            {
                MayDeleteUserData = true,
                Id = Guid.NewGuid()
            };

            await prov.UnprovisionAsync(Context);

            var output = LastIdentifiedOutput();
            Assert.Equal(prov.Id.ToStringInvariant(), output.Identifier);
            Assert.False(output.ObjectAdded);
            Assert.False(output.ObjectRemoved);
        }

        [FactNeedsSharePoint]
        public async Task Field_is_removed_by_id()
        {
            var field = await CreateField(f => f.Id);

            var prov = new HarshField()
            {
                MayDeleteUserData = true,
                Id = field.Id,
            };

            await prov.UnprovisionAsync(Context);

            var output = LastIdentifiedOutput();
            Assert.Equal(prov.Id.ToStringInvariant(), output.Identifier);
            Assert.False(output.ObjectAdded);
            Assert.True(output.ObjectRemoved);

            var fields = ClientContext.LoadQuery(Web.Fields.Include(f => f.Id));
            await ClientContext.ExecuteQueryAsync();

            Assert.DoesNotContain(field.Id, fields.Select(f => f.Id));
        }
    }
}
