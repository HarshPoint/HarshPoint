using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning
{
    public sealed class HarshContentTypeRef : HarshProvisioner
    {

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

            base.InitializeResolveContext(context);
        }

        protected override Task OnProvisioningAsync()
        {
            foreach (var list in Lists)
            {
                list.ContentTypesEnabled = true;
                list.Update();

                var existingCtIds = list.ContentTypes
                    .Select(HarshContentTypeId.Get)
                    .ToArray();

                var toAdd = ContentTypes
                    .Where(ct => !ContainsContentType(existingCtIds, ct));

                foreach (var ct in toAdd)
                {
                    list.ContentTypes.AddExistingContentType(ct);
                }
            }

            return ClientContext.ExecuteQueryAsync();
        }

        protected override Task OnUnprovisioningAsync()
        {
            var idsToRemove = ContentTypes
                .Select(HarshContentTypeId.Get)
                .ToArray();

            foreach (var list in Lists)
            {
                list.ContentTypesEnabled = true;
                list.Update();

                var toRemove = list.ContentTypes
                    .Where(ct => ContainsContentType(idsToRemove, ct));

                foreach (var ct in toRemove)
                {
                    ct.DeleteObject();
                }
            }

            return ClientContext.ExecuteQueryAsync();
        }

        private static Boolean ContainsContentType(IEnumerable<HarshContentTypeId> ids, ContentType ct)
        {
            var ctid = HarshContentTypeId.Get(ct);
            return ids.Any(id => ctid.IsDirectChildOf(id));
        }
    }
}
