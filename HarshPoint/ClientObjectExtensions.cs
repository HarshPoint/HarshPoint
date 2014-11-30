using Microsoft.SharePoint.Client;
using System;
using System.Linq.Expressions;

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

        public static Boolean IsPropertyAvailable<T>(this T clientObject, Expression<Func<T, Object>> expression)
            where T : ClientObject
        {
            if (clientObject == null)
            {
                throw Error.ArgumentNull("clientObject");
            }

            return clientObject.IsPropertyAvailable(expression.GetMemberName());
        }
    }
}
