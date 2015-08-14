using Microsoft.SharePoint.Client;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning
{
    public sealed class HarshRemoveContentTypeRef : HarshProvisioner
    {
        private readonly HarshContentTypeRef _inner = new HarshContentTypeRef()
        {
            MayDeleteUserData = true
        };

        public HarshRemoveContentTypeRef()
        {
            ForwardsTo(_inner);
        }

        [Parameter]
        public IResolve<ContentType> ContentTypes
        {
            get { return _inner.ContentTypes; }
            set { _inner.ContentTypes = value; }
        }

        [Parameter]
        [DefaultFromContext]
        public IResolve<List> Lists
        {
            get { return _inner.Lists; }
            set { _inner.Lists = value; }
        }
        
        protected override Task OnProvisioningAsync()
        {
            return _inner.UnprovisionAsync(Context);
        }
    }
}
