using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
    }
}
