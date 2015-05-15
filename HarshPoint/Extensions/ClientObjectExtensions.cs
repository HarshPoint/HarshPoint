using Microsoft.SharePoint.Client;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

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
                throw Error.ArgumentNull(nameof(clientObject));
            }

            if (expression == null)
            {
                throw Error.ArgumentNull(nameof(expression));
            }

            return clientObject.IsPropertyAvailable(expression.GetMemberName());
        }

        public static Task EnsurePropertyAvailable<T>(this T clientObject, Expression<Func<T, Object>> expression)
            where T : ClientObject
        {
            if (clientObject == null)
            {
                throw Error.ArgumentNull(nameof(clientObject));
            }

            if (expression == null)
            {
                throw Error.ArgumentNull(nameof(expression));
            }

            if (IsPropertyAvailable(clientObject, expression))
            {
                return HarshTask.Completed;
            }

            clientObject.Context.Load(clientObject, expression);
            return clientObject.Context.ExecuteQueryAsync();
        }
    }
}
