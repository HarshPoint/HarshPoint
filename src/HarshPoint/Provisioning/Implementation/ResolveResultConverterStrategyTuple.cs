using System;
using System.Collections.Immutable;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ResolveResultConverterStrategyTuple : ResolveResultConverterStrategy
    {
        private readonly Type _tupleType;
        private readonly IImmutableList<Type> _componentTypes;

        public ResolveResultConverterStrategyTuple(Type tupleType)
        {
            _tupleType = tupleType;
            _componentTypes = HarshTuple.GetComponentTypes(tupleType);
        }

        public override Object Convert(Object obj)
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

            var components = nested.ExtractComponents(_componentTypes);

            return HarshTuple.Create(_tupleType, components);
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<ResolveResultConverterStrategyTuple>();

    }
}
