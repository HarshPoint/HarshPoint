using HarshPoint.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HarshPoint.Linq
{
    public static class HarshQueryable
    {
        public static IEnumerable<Type> ExtractElementTypes(Type queryable)
            => queryable
                .ExtractGenericInterfaceTypeArguments(typeof(IQueryable<>))
                .Select(args => args[0]);
    }
}
