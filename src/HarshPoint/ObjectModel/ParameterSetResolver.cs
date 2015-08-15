using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace HarshPoint.ObjectModel
{
    internal sealed class ParameterSetResolver
    {
        private static readonly HarshLogger Logger = HarshLog.ForContext<ParameterSetResolver>();

        public ParameterSetResolver(Object target, IEnumerable<ParameterSet> parameterSets)
        {
            if (target == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(target));
            }

            if (parameterSets == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(parameterSets));
            }

            if (!parameterSets.Any())
            {
                throw Logger.Fatal.ArgumentEmptySequence(nameof(parameterSets));
            }

            DefaultParameterSet = parameterSets.Single(
                set => set.IsDefault
            );

            ParameterSets = parameterSets.ToImmutableDictionary(
                set => set.Name,
                ParameterSet.NameComparer
            );

            Target = target;
        }

        public ParameterSet Resolve()
        {
            if (ParameterSets.Count == 1)
            {
                Logger.Debug(
                    "Only one parameter set defined: {DefaultParameterSetName}",
                    DefaultParameterSet.Name
                );

                return DefaultParameterSet;
            }

            var candidates = Parameters.Aggregate(
                ParameterSets,
                RemoveCandidateIfParamDefault
            );

            if (candidates.Count == 0)
            {
                Logger.Debug(
                    "No parameter set matched, assuming default {DefaultParameterSetName}",
                    DefaultParameterSet.Name
                );

                return DefaultParameterSet;
            }

            if (candidates.Count == 1)
            {
                var result = candidates.Values.Single();

                Logger.Debug(
                    "Matched parameter set {ParameterSetName}",
                    result.Name
                );

                return result;
            }

            throw Logger.Fatal.InvalidOperation(SR.ParameterSetResolver_Ambiguous);
        }

        public ImmutableDictionary<String, ParameterSet> ParameterSets
        {
            get;

        }

        public Object Target
        {
            get;

        }

        private ParameterSet DefaultParameterSet
        {
            get;

        }

        private IEnumerable<Parameter> Parameters
            => ParameterSets.Values.SelectMany(set => set.Parameters);

        private ImmutableDictionary<String, ParameterSet> RemoveCandidateIfParamDefault(
            ImmutableDictionary<String, ParameterSet> candidates,
            Parameter parameter
        )
        {
            if (parameter.IsCommonParameter)
            {
                Logger.Debug(
                    "Skipping common parameter {Parameter}",
                    parameter.Name
                );

                return candidates;
            }

            if (parameter.HasDefaultValue(Target))
            {
                Logger.Debug(
                    "Parameter {Parameter} has default value, removing {ParameterSetName} from candidates",
                    parameter.Name,
                    parameter.ParameterSetName
                );

                return candidates.Remove(
                    parameter.ParameterSetName
                );
            }

            Logger.Debug(
                "Parameter {Parameter} has a value set, keeping {ParameterSetName} as a candidate",
                parameter.Name,
                parameter.ParameterSetName
            );

            return candidates;
        }
    }
}
