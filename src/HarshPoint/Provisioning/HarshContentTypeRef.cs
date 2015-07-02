using Microsoft.SharePoint.Client;
using System.Linq;
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

        [Parameter]
        [DefaultFromContext]
        public IResolve<List> Lists
        {
            get;
            set;
        }

        protected override async Task OnProvisioningAsync()
        {
            var contentTypes = await ResolveAsync(
                ContentTypes.Include(
                    ct => ct.Name,
                    ct => ct.StringId
                )
            );

            var lists = await ResolveAsync(
                Lists.Include(
                    list => list.ContentTypes.Include(
                        ct => ct.Name,
                        ct => ct.StringId
                    )
                )
            );

            foreach (var list in lists)
            {
                list.ContentTypesEnabled = true;
                list.Update();

                var existingCtIds = list.ContentTypes.Select(
                    ct => HarshContentTypeId.Parse(ct.StringId)
                );

                var toAdd = from ct in contentTypes
                            let id = HarshContentTypeId.Parse(ct.StringId)
                            where !existingCtIds.Any(existing => existing.IsDirectChildOf(id))
                            select ct;

                foreach (var ct in toAdd)
                {
                    list.ContentTypes.AddExistingContentType(ct);
                }
            }

            await ClientContext.ExecuteQueryAsync();
        }
    }
}
