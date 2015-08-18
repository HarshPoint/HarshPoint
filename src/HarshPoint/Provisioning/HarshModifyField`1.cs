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
        private ClientObjectUpdater _updater;

        protected HarshModifyField()
        {
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
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            context.Include(
                Updater.GetRetrievals<TField>()
            );

            base.InitializeResolveContext(context);
        }

        protected override async Task OnProvisioningAsync()
        {
            foreach (var field in Fields)
            {
                if (Updater.Update(field, this))
                {
                    UpdateField(field);
                }

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

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        protected virtual ClientObjectUpdater GetUpdater() => null;

        private ClientObjectUpdater GetUpdaterOrEmpty()
            => GetUpdater() ?? ClientObjectUpdater.Empty;

        private ClientObjectUpdater Updater
            => HarshLazy.Initialize(ref _updater, GetUpdaterOrEmpty);
    }
}
