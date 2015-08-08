using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HarshPoint.Shellploy
{
    internal sealed class CredentialFactory
    {
        public ICredentials CreateCredentials(CredentialType credentialType, string userName, string password, Uri url)
        {
            if (credentialType == CredentialType.Default
                && url != null
                && url.HostNameType == UriHostNameType.Dns
                && url.Host.EndsWith(".sharepoint.com", true, System.Globalization.CultureInfo.InvariantCulture))
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
    }
}
