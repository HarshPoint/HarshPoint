using System;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace HarshPoint.Tests
{
    internal class TheoryNeedsSharePointTestCase : XunitTheoryTestCase
    {
        [Obsolete("Deserialization only")]
        public TheoryNeedsSharePointTestCase() { }

        public TheoryNeedsSharePointTestCase(
            IMessageSink diagnosticSink,
            TestMethodDisplay defaultMethodDisplay,
            ITestMethod testMethod
        )
            : base(
                diagnosticSink, 
                defaultMethodDisplay, 
                testMethod
            )
        {
        }

        protected override String GetSkipReason(IAttributeInfo factAttribute)
        {
            if (!SharePointTestContext.IsAvailable)
            {
                return "This test requires a SharePoint connection. Please " +
                    "check your HarshPointTestUrl etc. environment variables.";
            }

            return base.GetSkipReason(factAttribute);
        }
    }
}