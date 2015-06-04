using Microsoft.SharePoint.Client;
using System.Linq;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning
{
    public class HarshFieldRef : HarshProvisioner
    {
        [DefaultFromContext]
        public IResolveSingle<ContentType> ContentType
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

        protected override async Task OnProvisioningAsync()
        {
            var fieldLinks = ClientContext.LoadQuery(
                ResolvedContentType.FieldLinks.Include(
                    fl => fl.Id
                )
            );

            await ClientContext.ExecuteQueryAsync();

            var flcis = from field in await ResolveAsync(Fields)
                        where !fieldLinks.Any(fl => fl.Id == field.Id)
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
        }

        private ContentType ResolvedContentType
        {
            get;
            set;
        }
    }
}
