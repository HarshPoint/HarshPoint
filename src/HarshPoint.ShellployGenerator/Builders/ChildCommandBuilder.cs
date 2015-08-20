using HarshPoint.Provisioning.Implementation;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;

namespace HarshPoint.ShellployGenerator.Builders
{
    internal class ChildCommandBuilder<TProvisioner, TParent> :
        IChildCommandBuilder
        where TProvisioner : HarshProvisionerBase
        where TParent : HarshProvisionerBase
    {
        private ImmutableDictionary<String, Object> _fixedParameters
            = ImmutableDictionary<String, Object>.Empty;

        private ImmutableHashSet<String> _ignoredParameters
            = ImmutableHashSet<String>.Empty;


        public ChildCommandBuilder()
        {
            Ignore(ShellployCommand.InputObjectPropertyName);
        }
        public Type ProvisionerType => typeof(TParent);

        public void SetFixedValue<TValue>(
            Expression<Func<TParent, TValue>> parameter,
            TValue value
        )
        {
            _fixedParameters = _fixedParameters.SetItem(
                parameter.ExtractLastPropertyAccess().Name, value
            );
        }

        public void Ignore(Expression<Func<TParent, Object>> parameter)
        {
            Ignore(parameter.ExtractLastPropertyAccess().Name);
        }

        public void Ignore(String parameter)
        {
            _ignoredParameters = _ignoredParameters.Add(
                parameter
            );
        }

        IEnumerable<ShellployCommandProperty> IChildCommandBuilder.Process(
            IEnumerable<ShellployCommandProperty> parentProperties
        )
        {
            var result = parentProperties
                .Where(p => !_ignoredParameters.Contains(p.Identifier))
                .ToImmutableArray();

            foreach (var property in result)
            {
                if (_fixedParameters.ContainsKey(property.Identifier))
                {
                    property.HasFixedValue = true;
                    property.FixedValue = _fixedParameters[property.Identifier];
                }
            }

            return result;
        }
    }
}