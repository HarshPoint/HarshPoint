using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HarshPoint.Tests.Provisioning
{
    public class RetrievalResolving : IClassFixture<SharePointClientFixture>
    {
        public RetrievalResolving(SharePointClientFixture fix)
        {
            Fixture = fix;
        }

        public SharePointClientFixture Fixture { get; private set; }

        [Fact]
        public async Task Multiple_equal_includes_dont_fail()
        {
            Fixture.ClientContext.Load(Fixture.Web, w => w.Id, w => w.Id);
            await Fixture.ClientContext.ExecuteQueryAsync();
        }

        [Fact]
        public async Task Title_gets_resolved_with_InternalName()
        {
            var ctx = new ClientObjectResolveContext()
            {
                ProvisionerContext = Fixture.Context
            };

            ctx.Include<Field>(
                f => f.InternalName,
                f => f.Description
            );

            var field = await Resolve.FieldById(HarshBuiltInFieldId.Title).ResolveSingleAsync(
                ctx                
            );

            Assert.NotNull(field);
            Assert.True(field.IsPropertyAvailable(f => f.Description));
            Assert.True(field.IsPropertyAvailable(f => f.InternalName));
            Assert.Equal("Title", field.InternalName);
        }
    }
}