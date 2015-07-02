﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace HarshPoint.Provisioning.Implementation
{
    [SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    public abstract class Resolvable<T, TIdentifier, TContext, TSelf> :
        Resolvable<T, TContext, TSelf>,
        IResolvableIdentifiers<TIdentifier>
        where T : class
        where TContext : HarshProvisionerContextBase
        where TSelf : Resolvable<T, TIdentifier, TContext, TSelf>
    {
        protected Resolvable(IEnumerable<TIdentifier> identifiers)
        {
            if (identifiers == null)
            {
                throw Error.ArgumentNull(nameof(identifiers));
            }

            Identifiers = identifiers.ToImmutableArray();
        }

        public IImmutableList<TIdentifier> Identifiers
        {
            get;
            private set;
        }

        protected override Object ToLogObject()
        {
            return Identifiers;
        }
    }
}