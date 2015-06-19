using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HarshPoint.Tests.Provisioning
{
    public abstract class TestFieldBase<TField, TProvisioner> :
        IClassFixture<SharePointClientFixture>
        where TField : Field
        where TProvisioner : HarshFieldProvisioner<TField>
    {
        protected TestFieldBase(SharePointClientFixture fixture, FieldType fieldType)
        {
            Fixture = fixture;
            FieldType = fieldType;
        }

        public FieldType FieldType { get; private set; }

        public SharePointClientFixture Fixture { get; private set; }

        protected Task RunWithField(TProvisioner provisioner, Action<TField> action)
        {
            return RunWithField(provisioner, f =>
            {
                action(f);
                return HarshTask.Completed;
            });
        }

        protected async Task RunWithField(TProvisioner provisioner, Func<TField, Task> action)
        {
            var guid = Guid.NewGuid();

            var field = new HarshField()
            {
                Type = FieldType,
                DisplayName = guid.ToString("n"),
                InternalName = guid.ToString("n"),
                Id = guid,
                Children = { provisioner },
            };

            await field.ProvisionAsync(Fixture.Context);

            try
            {
                await action(Fixture.ClientContext.CastTo<TField>(field.Field));
            }
            finally
            {
                field.Field.DeleteObject();
                await Fixture.ClientContext.ExecuteQueryAsync();
            }
        }
    }
}
