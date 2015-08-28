using HarshPoint.Diagnostics;
using Microsoft.SharePoint.Client;
using System;
using System.Net;

namespace HarshPoint.Tests
{
    internal static class SharePointTestContext
    {
        public static ClientContext Create()
        {
            var url = Environment.GetEnvironmentVariable("HarshPointTestUrl");

            if (String.IsNullOrWhiteSpace(url))
            {
                return new SeriloggedClientContext($"http://{Environment.MachineName}");
            }

            var clientContext = new SeriloggedClientContext(url);

            var username = Environment.GetEnvironmentVariable("HarshPointTestUser");
            var password = Environment.GetEnvironmentVariable("HarshPointTestPassword");
            var authType = Environment.GetEnvironmentVariable("HarshPointTestAuth");

            if (StringComparer.OrdinalIgnoreCase.Equals(authType, "Windows"))
            {
                clientContext.Credentials = new NetworkCredential(
                    username,
                    password
                );
            }
            else if (StringComparer.OrdinalIgnoreCase.Equals(authType, "SharePointOnline"))
            {
                clientContext.Credentials = new SharePointOnlineCredentials(
                    username,
                    password
                );
            }

            return clientContext;
        }

        static SharePointTestContext()
        {
            try
            {
                using (var ctx = SharePointTestContext.Create())
                {
                    ctx.ExecuteQueryAsync().Wait();
                    IsAvailable = true;
                }
            }
            catch
            {
            }
        }

        public static readonly Boolean IsAvailable;
    }
}
