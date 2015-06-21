using Microsoft.SharePoint.Client;
using System.Linq;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning
{
    public sealed class HarshRemoveContentTypeRef : HarshProvisioner
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
            var removeCtIds = (await TryResolveAsync(ContentTypes))
                .Select(ct => HarshContentTypeId.Parse(ct.StringId))
                .ToArray();

            var lists = await TryResolveAsync(Lists);

            foreach (var list in lists)
            {
                list.ContentTypesEnabled = true;

                var existingCts = ClientContext.LoadQuery(
                    list.ContentTypes.Include(ct => ct.StringId)
                );

                await ClientContext.ExecuteQueryAsync();

                var toRemove = from ct in existingCts
                               let id = HarshContentTypeId.Parse(ct.StringId)
                               where removeCtIds.Any(remove => id.IsDirectChildOf(remove))
                               select ct;

                foreach (var ct in toRemove)
                {
                    ct.DeleteObject();
                }

                list.Update();
            }

            await ClientContext.ExecuteQueryAsync();
        }
    }
}
