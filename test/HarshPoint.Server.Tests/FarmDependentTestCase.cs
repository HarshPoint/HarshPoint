using Microsoft.SharePoint.Administration;
using System;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace HarshPoint.Server.Tests
{
    public class FarmDependentTestCase : XunitTestCase
    {
        private static readonly Boolean FarmJoined;

        static FarmDependentTestCase()
        {
            try
            {
                FarmJoined = SPFarm.Joined;
            }
            catch { }
        }

        [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
        public FarmDependentTestCase() { }

        public FarmDependentTestCase(
            IMessageSink diagnosticMessageSink,
            TestMethodDisplay defaultMethodDisplay,
            ITestMethod testMethod,
            Object[] testMethodArguments = null
        )
            : base(diagnosticMessageSink, defaultMethodDisplay, testMethod, testMethodArguments)
        {
        }

        protected override String GetSkipReason(IAttributeInfo factAttribute)
        {
            if (!FarmJoined)
            {
                return "This test needs to run on a server joined to a SharePoint 2013 farm.";
            }

            return base.GetSkipReason(factAttribute);
        }
    }
}
