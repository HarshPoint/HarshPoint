using HarshPoint.Reflection;
using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HarshPoint
{
    internal static class ClientContextExtensions
    {
        private static readonly MethodInfo LoadQueryMethod = typeof(ClientContext)
            .GetRuntimeMethods()
            .First(m =>
                m.Name == "LoadQuery" &&
                m.IsGenericMethodDefinition &&
                m.GetParameters().Length == 1
            );

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ClientContextExtensions));

        public static IEnumerable LoadQuery(this ClientContext clientContext, IQueryable query)
        {
            if (clientContext == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(clientContext));
            }

            if (query == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(query));
            }

            var elementType = GetQueryableElementTypes(query.GetType()).FirstOrDefault();

            if (elementType == null)
            {
                throw Logger.Fatal.ArgumentNotAssignableTo(nameof(query), query, typeof(IQueryable<>));
            }

            return (IEnumerable)LoadQueryMethod
                .MakeGenericMethod(elementType)
                .Invoke(clientContext, new[] { query });
        }

        private static IEnumerable<Type> GetQueryableElementTypes(Type type)
            => from baseType in type.GetRuntimeBaseTypeChain()

               from interfaceType in baseType.ImplementedInterfaces
               where interfaceType.IsConstructedGenericType

               let interfaceTypeDef = interfaceType.GetGenericTypeDefinition()
               where interfaceTypeDef == typeof(IQueryable<>)

               select interfaceType.GenericTypeArguments[0];
    }
}
