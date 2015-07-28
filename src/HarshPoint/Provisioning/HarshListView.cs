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

            ExistingViews = DeferredResolveBuilder.Create(() =>
                Lists
                .AsClientObjectResolveBuilder()
                .View().ByUrl(Url)
                .As<Tuple<List, View>>()
            );
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
            foreach (var listView in ExistingViews)
            {
                var list = listView.Item1;
                var view = listView.Item2;

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

        private IResolve<Tuple<List, View>> ExistingViews
        {
            get; set;
        }

        private String InitialTitle
        {
            get { return HarshUrl.GetLeafWithoutExtension(Url); }
        }

    }
}
