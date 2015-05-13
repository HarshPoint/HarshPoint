using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System;
using System.Collections.Generic;

namespace HarshPoint.Server.Provisioning
{
    internal interface IHarshServerProvisionerContext
    {
        SPFarm Farm
        {
            get;
        }

        SPSite Site
        {
            get;
        }

        String UpgradeAction
        {
            get;
        }

        IReadOnlyDictionary<String, String> UpgradeArguments
        {
            get;
        }

        SPWeb Web
        {
            get;
        }

        SPWebApplication WebApplication
        {
            get;
        }
    }
}
