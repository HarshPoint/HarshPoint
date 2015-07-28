using Microsoft.SharePoint.Client;
using System.Linq;
using System.Threading.Tasks;
using HarshPoint.Provisioning.Implementation;

namespace HarshPoint.Provisioning
{
#warning NOT TESTED
    public sealed class HarshRemoveContentTypeRef : HarshProvisioner
    {
        public HarshRemoveContentTypeRef()
        {
            ExistingContentTypes = DeferredResolveBuilder.Create(
                () => Lists.AsClientObjectResolveBuilder()
                .ContentType()
                .As<IGrouping<List, ContentType>>()
            );
        }

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

        protected override void InitializeResolveContext(ClientObjectResolveContext context)
        {
            context.Include<List>(
                list => list.ContentTypes.Include(ct => ct.StringId)
            );

            base.InitializeResolveContext(context);
        }

        protected override async Task OnProvisioningAsync()
        {
            var removeCtIds =
                ContentTypes
                .Select(ct => HarshContentTypeId.Parse(ct.StringId))
                .ToArray();

            foreach (var listCts in ExistingContentTypes)
            {
                var list = listCts.Key;

                list.ContentTypesEnabled = true;

                var toRemove = from ct in listCts
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

        private IResolve<IGrouping<List,ContentType>> ExistingContentTypes
        {
            get; set;
        }
    }
}
