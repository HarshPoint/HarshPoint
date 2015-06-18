using Microsoft.SharePoint.Client;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning
{
    public class HarshListView : HarshProvisioner
    {
        public HarshListView()
        {
            ViewFields = new Collection<String>();
        }

        [DefaultFromContext]
        public IResolve<List> Lists
        {
            get;
            set;
        }

        public String Title
        {
            get;
            set;
        }

        public String Url
        {
            get;
            set;
        }

        public Collection<String> ViewFields
        {
            get;
            private set;
        }

        protected override async Task<HarshProvisionerResult> OnProvisioningAsync()
        {
            var lists = await ResolveAsync(
                Lists.Include(
                    l => l.RootFolder.ServerRelativeUrl,
                    l => l.Views.Include(
                        v => v.ServerRelativeUrl,
                        v => v.Title
                    )
                )
            );

            foreach (var list in lists)
            {
                var listRoot = list.RootFolder.ServerRelativeUrl;

            }

            throw new NotImplementedException();
        }

    }
}
