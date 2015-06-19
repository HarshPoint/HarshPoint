using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HarshPoint.Tests.Provisioning.Resolvers
{
    public class ListViewResolving : IClassFixture<SharePointClientFixture>
    {
        private const String ListTitle = "e156d2d63e5941b69e97f05ee1f92a13";
        private const String ViewTitle = "TestView";

        public ListViewResolving(SharePointClientFixture fixture)
        {
            Fixture = fixture;
        }

        public SharePointClientFixture Fixture { get; private set; }

        [Fact]
        public async Task View_get_resolved_by_Title()
        {
            var listAndView = await EnsureTestListAndView();

            IResolve<View> resolvable = Resolve
                .ListByUrl("Lists/" + ListTitle)
                .ViewByTitle(ViewTitle);

            var view = await resolvable
                .Include(v => v.Id)
                .ResolveSingleAsync(Fixture.ResolveContext);

            Assert.NotNull(view);
            Assert.Equal(listAndView.Item2.Id, view.Id);
        }

        [Fact]
        public async Task View_get_resolved_by_Url()
        {
            var listAndView = await EnsureTestListAndView();

            IResolve<View> resolvable = Resolve
                .ListByUrl("Lists/" + ListTitle)
                .ViewByUrl(ViewTitle + ".aspx");

            var view = await resolvable
                .Include(v => v.Id)
                .ResolveSingleAsync(Fixture.ResolveContext);

            Assert.NotNull(view);
            Assert.Equal(listAndView.Item2.Id, view.Id);
        }
        private async Task<Tuple<List, View>> EnsureTestListAndView()
        {
            var list = Fixture.Web.Lists.GetByTitle(ListTitle);

            try
            {
                await Fixture.ClientContext.ExecuteQueryAsync();
            }
            catch (ServerException)
            {
                list = Fixture.Web.Lists.Add(new ListCreationInformation()
                {
                    Url = "Lists/" + ListTitle,
                    Title = ListTitle,
                    TemplateType = (Int32)ListTemplateType.GenericList,
                });

                await Fixture.ClientContext.ExecuteQueryAsync();
            }

            var view = list.Views.GetByTitle(ViewTitle);
            Fixture.ClientContext.Load(view, v => v.Id);

            try
            {
                await Fixture.ClientContext.ExecuteQueryAsync();
            }
            catch (ServerException)
            {
                view = list.Views.Add(new ViewCreationInformation()
                {
                    Title = ViewTitle
                });

                Fixture.ClientContext.Load(view, v => v.Id);

                await Fixture.ClientContext.ExecuteQueryAsync();
            }

            return Tuple.Create(list, view);
        }
    }
}
