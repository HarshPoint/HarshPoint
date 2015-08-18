using HarshPoint.Provisioning.Implementation;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq.Expressions;

namespace HarshPoint.ShellployGenerator
{
    internal class ShellployCommandBuilderParent<TProvisioner, TParentProvisioner> : IShellployCommandBuilderParent
        where TProvisioner : HarshProvisionerBase
        where TParentProvisioner : HarshProvisionerBase
    {
        private ShellployCommandBuilder<TProvisioner> _builder;
        private Dictionary<String, Object> _fixedParameters
            = new Dictionary<String, Object>();
        private HashSet<String> _ignoredParameters = new HashSet<String>();

        public ShellployCommandBuilderParent(ShellployCommandBuilder<TProvisioner> builder)
        {
            _builder = builder;
        }

        public ShellployCommandBuilder<TProvisioner> End()
        {
            return _builder;
        }

        public ImmutableDictionary<String, Object> FixedParameters
        {
            get
            {
                return _fixedParameters.ToImmutableDictionary();
            }
        }

        public ImmutableHashSet<String> IgnoredParameters
        {
            get
            {
                return _ignoredParameters.ToImmutableHashSet();
            }
        }

        public Type Type
        {
            get
            {
                return typeof(TParentProvisioner);
            }
        }

        public ShellployCommandBuilderParent<TProvisioner, TParentProvisioner> SetValue<TValue>(
            Expression<Func<TParentProvisioner, TValue>> parameter,
            TValue value
        )
        {
            this._fixedParameters[parameter.ExtractLastPropertyAccess().Name] = value;

            return this;
        }

        public ShellployCommandBuilderParent<TProvisioner, TParentProvisioner> IgnoreParameter(
            Expression<Func<TParentProvisioner, Object>> parameter
        )
        {
            _ignoredParameters.Add(parameter.ExtractLastPropertyAccess().Name);
            return this;
        }
    }
}