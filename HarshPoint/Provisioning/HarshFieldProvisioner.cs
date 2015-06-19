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
        [DefaultFromContext]
        public IResolve<Field> Fields
        {
            get;
            set;
        }

        public Boolean NoPushChangesToLists
        {
            get;
            set;
        }

        protected override async Task InitializeAsync()
        {
            FieldsResolved = (await TryResolveAsync(Fields)).Cast<TField>();

            await base.InitializeAsync();
        }

        protected IEnumerable<TField> FieldsResolved
        {
            get;
            private set;
        }

        protected void UpdateField(TField field)
        {
            if (field == null)
            {
                throw Error.ArgumentNull(nameof(field));
            }

            field.UpdateAndPushChanges(!NoPushChangesToLists);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
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
