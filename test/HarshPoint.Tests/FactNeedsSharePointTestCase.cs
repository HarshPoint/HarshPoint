using System;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace HarshPoint.Tests
{
    internal class FactNeedsSharePointTestCase : XunitTestCase
    {
        [Obsolete("Deserialization only")]
        public FactNeedsSharePointTestCase()
        {
        }

        public FactNeedsSharePointTestCase(
            IMessageSink diagnosticSink,
            TestMethodDisplay defaultMethodDisplay,
            ITestMethod testMethod,
            Object[] testMethodArguments = null
        )
            : base(
                diagnosticSink,
                defaultMethodDisplay,
                testMethod,
                testMethodArguments
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