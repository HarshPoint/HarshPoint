using Microsoft.SharePoint.Client;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning
{
    public sealed class HarshContentTypeRef : HarshProvisioner
    {
        public IResolve<ContentType> ContentTypes
        {
            get;
            set;
        }

        [DefaultFromContext]
        public IResolve<List> Lists
        {
            get;
            set;
        }

        protected override async Task OnProvisioningAsync()
        {
            var contentTypes = await ResolveAsync(ContentTypes);
            var lists = await ResolveAsync(Lists);

            foreach (var list in lists)
            {
                list.ContentTypesEnabled = true;

                foreach (var ct in contentTypes)
                {
                    list.ContentTypes.AddExistingContentType(ct);
                }

                list.Update();
            }

            await ClientContext.ExecuteQueryAsync();
            await base.OnProvisioningAsync();
        }
    }
}
