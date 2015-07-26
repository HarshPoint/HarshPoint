using Microsoft.SharePoint.Client;
using System;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace HarshPoint.Tests
{
    internal static class ClientRequestExtension
    {
        public static String ToDiagnosticString(this ClientRequest request)
        {
            return ToDiagnosticXml(request).ToString(SaveOptions.OmitDuplicateNamespaces);
        }

        public static XDocument ToDiagnosticXml(this ClientRequest request)
        {
            if (request == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(request));
            }

            var chunkedSb = BuildQueryMethod.Invoke(request, null);

            using (var sw = new StringWriter())
            {
                WriteContentToMethod.Invoke(chunkedSb, new Object[] { sw });
                return XDocument.Parse(sw.ToString());
            }
        }

        private static readonly MethodInfo BuildQueryMethod = typeof(ClientRequest)
            .GetTypeInfo()
            .GetMethod("BuildQuery", BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly MethodInfo WriteContentToMethod = BuildQueryMethod
            .ReturnType
            .GetTypeInfo()
            .GetMethod("WriteContentTo", new[] { typeof(TextWriter) });

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ClientRequestExtension));
    }
}
