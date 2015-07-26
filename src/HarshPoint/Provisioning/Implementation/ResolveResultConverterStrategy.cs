using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace HarshPoint.Provisioning.Implementation
{
    internal abstract class ResolveResultConverterStrategy
    {
        private readonly Type _resultType;
        private readonly Lazy<IReadOnlyCollection<Type>> _componentsFlat;
            
        protected ResolveResultConverterStrategy()
        {
        }

        protected ResolveResultConverterStrategy(Type resultType)
        {
            if (resultType == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(resultType));
            }

            _resultType = resultType;
            _componentsFlat = new Lazy<IReadOnlyCollection<Type>>(GetComponentsFlat);
        }

        public virtual IEnumerable<Object> ConvertResults(IEnumerable<Object> results)
        {
            if (results == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(results));
            }

            return results.Select(ConvertObject);
        }

        protected virtual Object ConvertObject(Object obj)
        {
            var nested = (obj as INestedResolveResult);

            if (nested == null)
            {
                throw Logger.Fatal.ArgumentNotAssignableTo(
                    nameof(obj),
                    obj,
                    typeof(INestedResolveResult)
                );
            }

            return ConvertNested(nested);
        }

        protected virtual Object ConvertNested(INestedResolveResult nested)
        {
            var components = nested.ExtractComponents(ComponentTypesFlattened);

            using (var enumerator = components.AsEnumerable().GetEnumerator())
            {
                return ConvertNestedComponents(enumerator);
            }
        }

        protected virtual Object ConvertNestedComponents(IEnumerator<Object> componentEnumerator)
        {
            throw Logger.Fatal.NotImplemented();
        }

        protected IReadOnlyCollection<Type> ComponentTypesFlattened 
            => _componentsFlat?.Value ?? ImmutableArray<Type>.Empty;

        protected Type ResultType => _resultType;

        private IReadOnlyCollection<Type> GetComponentsFlat()
        {
            var result = ImmutableArray.CreateBuilder<Type>();
            AddComponentsFlat(result, ResultType);
            return result;
        }

        private static void AddComponentsFlat(ImmutableArray<Type>.Builder result, Type t)
        {
            if (HarshTuple.IsTupleType(t))
            {
                foreach (var ct in HarshTuple.GetComponentTypes(t))
                {
                    AddComponentsFlat(result, ct);
                }
            }
            else if (HarshGrouping.IsGroupingType(t))
            {
                AddComponentsFlat(result, t.GenericTypeArguments[0]);
                AddComponentsFlat(result, t.GenericTypeArguments[1]);
            }
            else
            {
                result.Add(t);
            }
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<ResolveResultConverterStrategy>();
    }
}
