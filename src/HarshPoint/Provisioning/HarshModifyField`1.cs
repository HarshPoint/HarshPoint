using HarshPoint.ObjectModel;
using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning
{
    public abstract class HarshModifyField<TField> : HarshProvisioner
        where TField : Field
    {
        private readonly ClientObjectUpdater<HarshModifyField<TField>, TField> _updater;

        public HarshModifyField()
        {
            _updater = new ClientObjectUpdater<HarshModifyField<TField>, TField>(
                Metadata
            );
        }

        [Parameter]
        [DefaultFromContext]
        public IResolve<TField> Fields
        {
            get;
            set;
        }

        [Parameter]
        public Boolean NoPushChangesToLists
        {
            get;
            set;
        }

        protected void UpdateField(TField field)
        {
            if (field == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(field));
            }

            field.UpdateAndPushChanges(!NoPushChangesToLists);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        protected void SetPropertyIfHasValue<T>(TField field, T? value, Expression<Func<TField, T>> property)
            where T : struct
        {
            if (field == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(field));
            }

            if (property == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(property));
            }

            if (value.HasValue)
            {
                property.ExtractLastPropertyAccess().SetValue(field, value.Value);
            }
        }


        protected override void InitializeResolveContext(ClientObjectResolveContext context)
        {
            context.Include(
                _updater.GetRetrievals()
            );

            base.InitializeResolveContext(context);
        }

        protected override async Task OnProvisioningAsync()
        {
            foreach (var field in Fields)
            {
                ModifyField(field);
            }

            if (ClientContext.HasPendingRequest)
            {
                await ClientContext.ExecuteQueryAsync();
            }
        }

        protected virtual void ModifyField(TField field)
        {
        }
    }
}
