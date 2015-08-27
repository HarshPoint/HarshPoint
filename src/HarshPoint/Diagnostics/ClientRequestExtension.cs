using Microsoft.SharePoint.Client;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace HarshPoint.Diagnostics
{
    internal static class ClientRequestExtension
    {
        public static String ToDiagnosticString(this ClientRequest request)
            => ToDiagnosticXml(request).ToString(SaveOptions.OmitDuplicateNamespaces);

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
            .GetDeclaredMethod("BuildQuery");

        private static readonly MethodInfo WriteContentToMethod = BuildQueryMethod
            .ReturnType
            .GetTypeInfo()
            .GetDeclaredMethods("WriteContentTo")
            .Select(m => new { m, p = m.GetParameters() })
            .FirstOrDefault(
                x => x.p.Length == 1 && x.p[0].ParameterType == typeof(TextWriter)
            )?.m;

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ClientRequestExtension));
    }
}
