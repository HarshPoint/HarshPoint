using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using System;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning
{
    public abstract class TestFieldBase<TField, TProvisioner> :
        SharePointClientTest
        where TField : Field
        where TProvisioner : HarshModifyField<TField>
    {
        protected TestFieldBase(FieldType fieldType, SharePointClientFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
            FieldType = fieldType;
        }

        public FieldType FieldType { get; private set; }

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
                InternalName = guid.ToString("n"),
                Id = guid,
                Children = { provisioner },
            };

            await field.ProvisionAsync(Context);

            var fo = FindOutput<Field>();

            try
            {
                await action(ClientContext.CastTo<TField>(fo.Object));
            }
            finally
            {
                fo.Object.DeleteObject();
                await ClientContext.ExecuteQueryAsync();
            }
        }
    }
}
