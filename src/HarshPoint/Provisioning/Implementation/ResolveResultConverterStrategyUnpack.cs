using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ResolveResultConverterStrategyUnpack : ResolveResultConverterStrategy
    {
        private ResolveResultConverterStrategyUnpack() { }

        public override Object ConvertObject(Object obj)
            => TagClientObject(
                NestedResolveResult.Unpack(obj),
                obj as NestedResolveResult
            );

        /// <remarks>This method is used by <see cref="ResolveResultConverterStrategyGrouping"/>
        /// to fetch simple (non-tuple) types.</remarks>
        public override Object ConvertNestedComponents(
            NestedResolveResult nested,
            IEnumerator<Object> componentEnumerator
        ) 
            => TagClientObject(
                componentEnumerator.GetNextOrFail(),
                nested
            );

        private static Object TagClientObject(
            Object result,
            NestedResolveResult nested
        )
        {
            var clientObject = (result as ClientObject);

            if (clientObject != null && clientObject.Tag == null)
            {
                clientObject.Tag = nested;
            }

            return result;
        }

        public static ResolveResultConverterStrategy Instance { get; }
            = new ResolveResultConverterStrategyUnpack();
    }
}
