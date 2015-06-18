using Microsoft.SharePoint.Client;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning
{
    public class HarshFieldRef : HarshProvisioner
    {
        [DefaultFromContext]
        public IResolve<ContentType> ContentType
        {
            get;
            set;
        }

        public IResolve<Field> Fields
        {
            get;
            set;
        }

        public Boolean Hidden
        {
            get;
            set;
        }

        public Boolean Required
        {
            get;
            set;
        }

        protected override async Task InitializeAsync()
        {
            ResolvedContentType = await ResolveSingleAsync(ContentType);
        }

        protected override async Task<HarshProvisionerResult> OnProvisioningAsync()
        {
            var existingLinks = ClientContext.LoadQuery(
                ResolvedContentType.FieldLinks.Include(
                    fl => fl.Id,
                    fl => fl.Name
                )
            );

            await ClientContext.ExecuteQueryAsync();

            var fields = await ResolveAsync(
                Fields,
                f => f.Id
            );

            var links = from field in fields
                        select
                            existingLinks.FirstOrDefault(fl => fl.Id == field.Id) 
                            ?? 
                            ResolvedContentType.FieldLinks.Add(
                                new FieldLinkCreationInformation() { Field = field }
                            );

            foreach (var link in links)
            {
                link.Hidden = Hidden;
                link.Required = Required;
            }

            ResolvedContentType.Update(updateChildren: true);
            await ClientContext.ExecuteQueryAsync();

            return await base.OnProvisioningAsync();
        }

        private ContentType ResolvedContentType
        {
            get;
            set;
        }
    }
}
