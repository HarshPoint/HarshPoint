using Microsoft.SharePoint.Client;
using System;
using System.Linq.Expressions;

namespace HarshPoint.Provisioning.Implementation
{
    partial class ClientObjectQueryProcessor
    {
        private sealed class MethodCallInfo
        {
            public MethodCallInfo(MethodCallExpression node)
            {
                if (node == null)
                {
                    return;
                }

                if (!node.Method.DeclaringType.Equals(typeof(ClientObjectQueryableExtension)))
                {
                    return;
                }

                if (node.Arguments.Count != 2)
                {
                    return;
                }

                if (!node.Method.IsGenericMethod)
                {
                    return;
                }

                var genericArguments = node.Method.GetGenericArguments();

                if (genericArguments.Length != 1)
                {
                    return;
                }

                IsIncludeWithDefaultProperties = 
                    node.Method.Name.Equals("IncludeWithDefaultProperties");

                IsInclude = 
                    IsIncludeWithDefaultProperties || 
                    node.Method.Name.Equals("Include");

                if (!IsInclude)
                {
                    return;
                }

                ElementType = genericArguments[0];
            }

            public Type ElementType
            {
                get;
                private set;
            }

            public Boolean IsInclude
            {
                get;
                private set;
            }

            public Boolean IsIncludeWithDefaultProperties
            {
                get;
                private set;
            }
        }

    }
}