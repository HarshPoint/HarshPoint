using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System.Linq;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning
{
    public sealed class HarshContentTypeRef : HarshProvisioner
    {
        public HarshContentTypeRef()
        {
        }

        [Parameter(Mandatory = true)]
        public IResolve<ContentType> ContentTypes
        {
            get;
            set;
        }

        [DefaultFromContext]
        [Parameter(Mandatory = true)]
        public IResolve<List> Lists
        {
            get;
            set;
        }

        protected override void InitializeResolveContext(ClientObjectResolveContext context)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            context.Include<ContentType>(
                ct => ct.Name,
                ct => ct.StringId
            );

            context.Include<List>(
                list => list.ContentTypes
            );
        }

        protected override async Task OnProvisioningAsync()
        {
            foreach (var list in Lists)
            {
                list.ContentTypesEnabled = true;
                list.Update();

                var existingCtIds = list.ContentTypes.Select(
                    ct => HarshContentTypeId.Parse(ct.StringId)
                );

                var toAdd = from ct in ContentTypes
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
