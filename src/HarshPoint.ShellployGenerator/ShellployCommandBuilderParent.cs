using HarshPoint.Provisioning.Implementation;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace HarshPoint.ShellployGenerator
{
    internal class ShellployCommandBuilderParent<TProvisioner, TParentProvisioner> : IShellployCommandBuilderParent
        where TProvisioner : HarshProvisionerBase
        where TParentProvisioner : HarshProvisionerBase
    {
        private Dictionary<String, object> _fixedParameters
            = new Dictionary<String, object>();

        public Dictionary<String, object> FixedParameters
        {
            get
            {
                return _fixedParameters;
            }
        }

        public Type Type
        {
            get
            {
                return typeof(TParentProvisioner);
            }
        }

        public ShellployCommandBuilderParent<TProvisioner, TParentProvisioner> AddFixedParameter<TValue>(
            Expression<Func<TParentProvisioner, TValue>> parameter,
            TValue value
        )
        {
            this._fixedParameters[parameter.ExtractSinglePropertyAccess().Name] = value;

            return this;
        }

    }
}