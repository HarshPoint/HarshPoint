using Microsoft.SharePoint.Client;
using System;
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

        [DefaultFromContext]
        public IResolve<List> Lists
        {
            get;
            set;
        }

        protected override async Task<HarshProvisionerResult> OnProvisioningAsync()
        {
            var contentTypes = await ResolveAsync(ContentTypes);
            var lists = await ResolveAsync(Lists); // todo: Include support on resolvers!!!

            foreach (var list in lists)
            {
                list.ContentTypesEnabled = true;

                var existingCts = ClientContext.LoadQuery(
                    list.ContentTypes.Include(ct => ct.StringId)
                );

                await ClientContext.ExecuteQueryAsync();

                var existingCtIds = existingCts.Select(ct => HarshContentTypeId.Parse(ct.StringId));

                var toAdd = from ct in contentTypes
                            let id = HarshContentTypeId.Parse(ct.StringId)
                            where !existingCtIds.Any(existing => existing.IsDirectChildOf(id))
                            select ct;

                foreach (var ct in toAdd)
                {
                    list.ContentTypes.AddExistingContentType(ct);
                }

                list.Update();
            }

            await ClientContext.ExecuteQueryAsync();
            return await base.OnProvisioningAsync();
        }
    }
}
