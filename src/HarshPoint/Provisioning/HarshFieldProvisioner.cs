using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using HarshPoint.Reflection;
using System.Diagnostics.CodeAnalysis;

namespace HarshPoint.Provisioning
{
    public abstract class HarshFieldProvisioner<TField> : HarshProvisioner
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
        protected void SetPropertyIfHasValue<T>(TField field, Nullable<T> value, Expression<Func<TField, T>> property)
            where T : struct
        {
            if (value.HasValue)
            {
                var setter = property
                    .ExtractSinglePropertyAccess()
                    .MakeSetter<TField, T>();

                setter(field, value.Value);
            }
        }
    }
}
