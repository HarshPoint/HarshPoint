using Microsoft.SharePoint.Client;
using System;
using System.Threading.Tasks;

namespace HarshPoint.Diagnostics
{
    public sealed class SeriloggedClientContext : ClientContext
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
            catch (Exception exc)
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
