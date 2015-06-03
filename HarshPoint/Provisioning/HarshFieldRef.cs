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

        [DefaultFromContext]
        public IResolve<Field> Fields
        {
            get;
            set;
        }

        protected override async Task InitializeAsync()
        {
            ResolvedContentType = await ResolveSingleOrDefaultAsync(ContentType);
        }

        protected override async Task OnProvisioningAsync()
        {
            var flcis = from field in await ResolveAsync(Fields)
                        select new FieldLinkCreationInformation()
                        {
                            Field = field,
                        };

            foreach (var flci in flcis)
            {
                ResolvedContentType.FieldLinks.Add(flci);
            }

            await ClientContext.ExecuteQueryAsync();
        }

        private ContentType ResolvedContentType
        {
            get;
            set;
        }
    }
}
