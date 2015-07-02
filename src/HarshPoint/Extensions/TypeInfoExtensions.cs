using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace HarshPoint
{
    public static class TypeInfoExtensions
    {
        public static String GetCSharpSimpleName(this TypeInfo typeInfo)
        {
            if (typeInfo == null)
            {
                throw Error.ArgumentNull(nameof(typeInfo));
            }

            var nameWithoutArgCount = Regex.Replace(typeInfo.Name, "`\\d+$", String.Empty);
            var result = new StringBuilder(nameWithoutArgCount);

            if (typeInfo.IsGenericTypeDefinition)
            {
                AppendGenericArgumentsOrParameters(
                    result,
                    typeInfo.GenericTypeParameters
                );

            }
            else if (typeInfo.IsGenericType)
            {
                AppendGenericArgumentsOrParameters(
                    result,
                    typeInfo.GenericTypeArguments
                );
            }

            return result.ToString();
        }

        private static void AppendGenericArgumentsOrParameters(StringBuilder result, IEnumerable<Type> argsOrParams)
        {
            if (!argsOrParams.Any())
            {
                return;
            }

            const String Delimiter = ", ";

            result.Append('<');

            foreach (var item in argsOrParams)
            {
                result.Append(GetCSharpSimpleName(item.GetTypeInfo()));
                result.Append(Delimiter);
            }

            result.Length -= Delimiter.Length;
            result.Append('>');
        }
    }
}
