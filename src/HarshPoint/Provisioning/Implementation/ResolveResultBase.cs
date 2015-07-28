using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace HarshPoint.Provisioning.Implementation
{
    internal abstract class ResolveResultBase : IResolveBuilder
    {
        internal IResolveBuilder ResolveBuilder
        {
            get;
            set;
        }

        internal IEnumerable Results
        {
            get;
            set;
        }

        protected IImmutableList<T> EnumerateResults<T>()
            => (Results ?? new T[0]).Cast<T>().ToImmutableArray();

        Object IResolveBuilder.Initialize(IResolveContext context)
        {
            throw Logger.Fatal.NotImplemented();
        }

        void IResolveBuilder.InitializeContext(IResolveContext context)
        {
            throw Logger.Fatal.NotImplemented();
        }

        IEnumerable<Object> IResolveBuilder.ToEnumerable(IResolveContext context, Object state)
        {
            throw Logger.Fatal.NotImplemented();
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<ResolveResultBase>();
    }
}
