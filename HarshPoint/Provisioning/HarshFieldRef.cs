using Microsoft.SharePoint.Client;
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

        protected override async Task InitializeAsync()
        {
            ResolvedContentType = await ResolveSingleAsync(ContentType);
        }

        protected override async Task<HarshProvisionerResult> OnProvisioningAsync()
        {
            var existingFieldLinks = ClientContext.LoadQuery(
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

            var flcis = from field in fields
                        where !existingFieldLinks.Any(fl => fl.Id == field.Id)
                        select new FieldLinkCreationInformation()
                        {
                            Field = field,
                        };

            foreach (var flci in flcis)
            {
                ResolvedContentType.FieldLinks.Add(flci);
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
