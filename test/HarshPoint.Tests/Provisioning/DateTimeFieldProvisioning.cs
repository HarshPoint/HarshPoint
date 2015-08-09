using HarshPoint.Provisioning;
using Microsoft.SharePoint.Client;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning
{
    public class DateTimeFieldProvisioning : TestFieldBase<FieldDateTime, HarshModifyFieldDateTime>
    {
        public DateTimeFieldProvisioning(ITestOutputHelper output) 
            : base(FieldType.DateTime, output)
        {
        }

        [Fact]
        public async Task DisplayFormat_is_set()
        {
            var prov = new HarshModifyFieldDateTime()
            {
                DisplayFormat = DateTimeFieldFormatType.DateOnly,
            };

            await RunWithField(prov, f =>
            {
                Assert.Equal(DateTimeFieldFormatType.DateOnly, f.DisplayFormat);
            });
        }
    }
}
