using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning.Resolvers
{
    public class ListViewResolving : SharePointClientTest
    {
        private const String ViewTitle = "TestView";

        public ListViewResolving(ITestOutputHelper output) : base(output)
        {
        }

        [FactNeedsSharePoint]
        public async Task View_get_resolved_by_Title()
        {
            var listAndView = await EnsureTestListAndView();

            var resolvable = ManualResolver.ResolveSingle(
                Resolve.List().ById(listAndView.Item1.Id).View().ByTitle(ViewTitle),
                v => v.Id
            );

            await ClientContext.ExecuteQueryAsync();
            var view = resolvable.Value;

            Assert.NotNull(view);
            Assert.Equal(listAndView.Item2.Id, view.Id);
        }

        [FactNeedsSharePoint]
        public async Task View_get_resolved_by_Url()
        {
            var listAndView = await EnsureTestListAndView();

            var resolvable = ManualResolver.ResolveSingle(
                Resolve.List().ById(listAndView.Item1.Id).View().ByUrl(ViewTitle + ".aspx"),
                v => v.Id
            );

            await ClientContext.ExecuteQueryAsync();
            var view = resolvable.Value;

            Assert.NotNull(view);
            Assert.Equal(listAndView.Item2.Id, view.Id);
        }
        private async Task<Tuple<List, View>> EnsureTestListAndView()
        {
            var list = await CreateList();

            var view = list.Views.GetByTitle(ViewTitle);
            ClientContext.Load(view, v => v.Id);

            try
            {
                await ClientContext.ExecuteQueryAsync();
            }
            catch (ServerException)
            {
                view = list.Views.Add(new ViewCreationInformation()
                {
                    Title = ViewTitle
                });

                ClientContext.Load(view, v => v.Id);

                await ClientContext.ExecuteQueryAsync();
            }

            return Tuple.Create(list, view);
        }
    }
}
