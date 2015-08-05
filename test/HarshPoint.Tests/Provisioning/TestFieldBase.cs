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
            var ctx = Fixture.CreateContext();

            var field = new HarshFieldProvisioner()
            {
                Type = FieldType,
                DisplayName = guid.ToString("n"),
                InternalName = guid.ToString("n"),
                Id = guid,
                Children = { provisioner },
            };

            await field.ProvisionAsync(ctx);

            var fo = FindOutput<Field>();

            try
            {

                await action(ctx.ClientContext.CastTo<TField>(fo.Object));
            }
            finally
            {
                fo.Object.DeleteObject();
                await ctx.ClientContext.ExecuteQueryAsync();
            }
        }
    }
}
