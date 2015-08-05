using Microsoft.SharePoint.Client;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace HarshPoint.Provisioning
{
    public abstract class HarshModifyField<TField> : HarshProvisioner
        where TField : Field
    {
        [Parameter]
        [DefaultFromContext]
        public IResolve<TField> Fields
        {
            get;
            set;
        }

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
                property.ExtractSinglePropertyAccess().SetValue(field, value.Value);
            }
        }
    }
}
