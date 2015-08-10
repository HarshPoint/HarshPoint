using Microsoft.SharePoint.Client;
using System;
using System.Net;
using System.Threading.Tasks;

namespace HarshPoint.Tests
{
    public class SharePointClientFixture : IDisposable
    {
        public const String TestListUrl = "HarshPoint Tests";
        public const String TestListTitle = "HarshPoint Tests";

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

        [Obsolete("tests run in parallel, dependencies suck")]
        public async Task<List> EnsureTestList()
        {
            var list = ClientContext.Web.Lists.GetByTitle(TestListTitle);
            ClientContext.Load(list);

            try
            {
                await ClientContext.ExecuteQueryAsync();
            }
            catch (ServerException) 
            {
                list = ClientContext.Web.Lists.Add(new ListCreationInformation()
                {
                    Url = TestListUrl,
                    Title = TestListTitle,
                    TemplateType = (Int32)ListTemplateType.DocumentLibrary,
                    DocumentTemplateType = (Int32)DocumentTemplateType.Word,
                });

                try
                {
                    await ClientContext.ExecuteQueryAsync();
                }
                catch (ServerException)
                {
                    // most likely another test created the list between our 
                    // attempt to load it and the exception being caught
                }
            }

            return list;
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
                _pendingRequestBody = PendingRequest.ToDiagnosticString();
                return base.ExecuteQueryAsync();
            }

            protected override void OnExecutingWebRequest(WebRequestEventArgs args)
            {
                Serilog.Log.Information(
                    "{Method:l} {Uri:l}\n{Body:l}",
                    args.WebRequest.Method,
                    args.WebRequest.RequestUri,
                    _pendingRequestBody
                );

                _pendingRequestBody = null;
                base.OnExecutingWebRequest(args);
            }
        }
    }
}
