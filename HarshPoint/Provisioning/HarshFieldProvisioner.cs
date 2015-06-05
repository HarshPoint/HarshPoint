using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning
{
    public abstract class HarshFieldProvisioner<TField> : HarshProvisioner
        where TField : Field
    {
        [DefaultFromContext]
        public IResolve<Field> Field
        {
            get;
            set;
        }

        protected override async Task InitializeAsync()
        {
            FieldResolved = (await ResolveAsync(Field)).Cast<TField>();

            await base.InitializeAsync();
        }

        private IEnumerable<TField> FieldResolved
        {
            get;
            set;
        }
    }
}
