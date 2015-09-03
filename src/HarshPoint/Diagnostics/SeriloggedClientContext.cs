using Microsoft.SharePoint.Client;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace HarshPoint.Diagnostics
{
    public class SeriloggedClientContext : ClientContext
    {
        private String _pendingRequestBody;

        public SeriloggedClientContext(Uri webFullUrl) : base(webFullUrl)
        {
        }

        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#")]
        public SeriloggedClientContext(String webFullUrl) : base(webFullUrl)
        {
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
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
            if (args == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(args));
            }

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
