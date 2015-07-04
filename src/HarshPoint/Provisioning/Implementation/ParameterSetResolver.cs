using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ParameterSetResolver
    {
        private static readonly ILogger Logger = Log.ForContext<ParameterSetResolver>();

        public ParameterSetResolver(Object target, IEnumerable<ParameterSetMetadata> parameterSets)
        {
            if (target == null)
            {
                throw Error.ArgumentNull(nameof(target));
            }

            if (parameterSets == null)
            {
                throw Error.ArgumentNull(nameof(parameterSets));
            }

            if (!parameterSets.Any())
            {
                throw Error.ArgumentOutOfRange_EmptySequence(nameof(parameterSets));
            }

            DefaultParameterSet = parameterSets.Single(
                set => set.IsDefault
            );

            ParameterSets = parameterSets.ToImmutableDictionary(
                set => set.Name,
                ParameterSetMetadata.NameComparer
            );
            Target = target;
        }

        public ParameterSetMetadata Resolve()
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

            throw Error.InvalidOperation(SR.ParameterSetResolver_Ambiguous);
        }

        public ImmutableDictionary<String, ParameterSetMetadata> ParameterSets
        {
            get;
            private set;
        }

        public Object Target
        {
            get;
            private set;
        }

        private ParameterSetMetadata DefaultParameterSet
        {
            get;
            set;
        }

        private IEnumerable<ParameterMetadata> Parameters
            => ParameterSets.Values.SelectMany(set => set.Parameters);

        private ImmutableDictionary<String, ParameterSetMetadata> RemoveCandidateIfParamDefault(
            ImmutableDictionary<String, ParameterSetMetadata> candidates,
            ParameterMetadata parameter
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
