using Microsoft.SharePoint.Client;
using System;

namespace HarshPoint
{
    internal static class ClientObjectExtensions
    {
        public static Boolean IsNull(this ClientObject clientObject)
        {
            if (clientObject == null)
            {
                return true;
            }

            if (clientObject.ServerObjectIsNull.HasValue)
            {
                return clientObject.ServerObjectIsNull.Value;
            }

            return false;
        }
    }
}
