using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HarshPoint.Shellploy
{
    internal static class CredentialFactory
    {
        public static ICredentials CreateCredentials(
            CredentialType credentialType,
            String userName,
            String password,
            Uri url
        )
        {
            if (credentialType == CredentialType.Default
                && IsSharePointOnline(url)
            )
            {
                credentialType = CredentialType.SharePointOnline;
            }

            switch (credentialType)
            {
                case CredentialType.Windows:
                    return new NetworkCredential(
                        userName,
                        password
                    );
                case CredentialType.SharePointOnline:
                    return new SharePointOnlineCredentials(
                        userName,
                        password
                    );
            }

            return null;
        }

        private static Boolean IsSharePointOnline(Uri url)
            => url != null
                && url.HostNameType == UriHostNameType.Dns
                && url.Host.EndsWith(".sharepoint.com", StringComparison.OrdinalIgnoreCase);
    }
}
