using Microsoft.SharePoint.Client;
using System;
using System.Net;
using System.Threading.Tasks;

namespace HarshPoint.Tests
{
    public class SharePointClientFixture : IDisposable
    {
        public SharePointClientFixture()
        {
            var url = Environment.GetEnvironmentVariable("HarshPointTestUrl");

            if (String.IsNullOrWhiteSpace(url))
            {
                ClientContext = new SeriloggedClientContext($"http://{Environment.MachineName}");
            }
            else
            {
                ClientContext = new SeriloggedClientContext(url);

                var username = Environment.GetEnvironmentVariable("HarshPointTestUser");
                var password = Environment.GetEnvironmentVariable("HarshPointTestPassword");
                var authType = Environment.GetEnvironmentVariable("HarshPointTestAuth");

                if (StringComparer.OrdinalIgnoreCase.Equals(authType, "Windows"))
                {
                    ClientContext.Credentials = new NetworkCredential(
                        username,
                        password
                    );
                }
                else if (StringComparer.OrdinalIgnoreCase.Equals(authType, "SharePointOnline"))
                {
                    ClientContext.Credentials = new SharePointOnlineCredentials(
                        username,
                        password
                    );
                }
            }
        }

        public void Dispose()
        {
            ClientContext?.Dispose();
            ClientContext = null;
        }

        public ClientContext ClientContext
        {
            get;
            set;
        }

        private sealed class SeriloggedClientContext : ClientContext
        {
            private String _pendingRequestBody;

            public SeriloggedClientContext(Uri webFullUrl) : base(webFullUrl)
            {
            }

            public SeriloggedClientContext(String webFullUrl) : base(webFullUrl)
            {
            }

            public override Task ExecuteQueryAsync()
            {
                try
                {
                    _pendingRequestBody = PendingRequest.ToDiagnosticString();
                }
                catch(Exception exc)
                {
                    Logger.Error.Write(exc);
                }

                return base.ExecuteQueryAsync();
            }

            protected override void OnExecutingWebRequest(WebRequestEventArgs args)
            {
                Logger.Information(
                    "{Method:l} {Uri:l}\n{Body:l}",
                    args.WebRequest.Method,
                    args.WebRequest.RequestUri,
                    _pendingRequestBody
                );

                _pendingRequestBody = null;
                base.OnExecutingWebRequest(args);
            }

            private static readonly HarshLogger Logger = HarshLog.ForContext<SeriloggedClientContext>();
        }
    }
}
