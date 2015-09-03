using HarshPoint.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HarshPoint.Linq
{
    public static class HarshQueryable
    {
        public static Boolean IsQueryable(Type type)
        {
            if (type == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(type));
            }

            return ExtractElementTypes(type).Any();
        }

        public static IEnumerable<Type> ExtractElementTypes(Type queryable)
        {
            if (queryable == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(queryable));
            }

            return queryable
                .ExtractGenericInterfaceTypeArguments(typeof(IQueryable<>))
                .Select(args => args[0]);
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(HarshQueryable));
    }
}
