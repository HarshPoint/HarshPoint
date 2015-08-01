using HarshPoint.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HarshPoint.Linq
{
    public static class HarshEnumerable
    {
        public static IEnumerable<Type> ExtractElementTypes(Type enumerable)
            => enumerable
                .ExtractGenericInterfaceTypeArguments(typeof(IEnumerable<>))
                .Select(args => args[0]);
    }
}
