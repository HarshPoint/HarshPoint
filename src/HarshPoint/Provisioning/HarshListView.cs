using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
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

        [Parameter]
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

        protected override async Task OnProvisioningAsync()
        {
            var lists = await ResolveAsync(
                (IResolve<IGrouping<List, View>>)Lists.ViewByUrl(Url)
            );

            foreach (var grouping in lists)
            {
                var list = grouping.Key;
                var view = grouping.SingleOrDefault();

                if (view == null)
                {
                    view = list.Views.Add(new ViewCreationInformation()
                    {
                        Title = InitialTitle,
                        ViewFields = ViewFields.ToArray(),
                    });
                }

                view.Title = Title;
                view.Update();
            }

            await ClientContext.ExecuteQueryAsync();
        }

        private String InitialTitle
        {
            get { return HarshUrl.GetLeafWithoutExtension(Url); }
        }

    }
}
