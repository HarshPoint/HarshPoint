using HarshPoint.ObjectModel;
using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning
{
    public abstract class HarshModifyField<TField, TSelf> : HarshProvisioner
        where TField : Field
        where TSelf : HarshModifyField<TField, TSelf>
    {
        private LazyObjectMapping<TSelf, TField> _map;

        protected HarshModifyField()
        {
            Map(f => f.Title);
            Map(f => f.Group);

            WriteRecord = CreateRecordWriter<TField>();
        }

        [Parameter]
        public String Title { get; set; }

        [Parameter]
        [DefaultFromContext]
        public IResolve<TField> Fields { get; set; }

        [Parameter]
        [DefaultFromContext(typeof(DefaultFieldGroup))]
        public String Group { get; set; }

        [Parameter]
        public Boolean NoPushChangesToLists { get; set; }

        private RecordWriter<HarshProvisionerContext, TField> WriteRecord { get; }

        protected ObjectMapper<TSelf, TField>.IEntryBuilder Map(
            Expression<Func<TField, Object>> targetProperty
        )
            => _map.Map(targetProperty);

        protected void UpdateField(TField field)
        {
            if (field == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(field));
            }

            field.UpdateAndPushChanges(!NoPushChangesToLists);
        }

        protected override void InitializeResolveContext(
            ClientObjectResolveContext context
        )
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            context.Include<TField>(f => f.InternalName);

            context.Include(
                _map.GetTargetExpressions()
            );

            base.InitializeResolveContext(context);
        }

        protected override async Task OnProvisioningAsync()
        {
            foreach (var field in Fields)
            {
                if(_map.Apply(WriteRecord, this, field))
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
    }
}
