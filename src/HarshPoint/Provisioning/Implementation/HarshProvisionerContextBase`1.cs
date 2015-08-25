using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace HarshPoint.Provisioning.Implementation
{
    public abstract class HarshProvisionerContextBase<TSelf> :
        IHarshProvisionerContext, IHarshCloneable
        where TSelf : HarshProvisionerContextBase<TSelf>
    {
        private Boolean _mayDeleteUserData;
        private IProgress<HarshProvisionerRecord> _progress;
        private ImmutableStack<Object> _stateStack
            = ImmutableStack<Object>.Empty;

        internal HarshProvisionerContextBase() { }

        public Boolean MayDeleteUserData => _mayDeleteUserData;

        public TSelf AllowDeleteUserData()
            => (TSelf)this.With(c => c._mayDeleteUserData, true);

        public virtual TSelf Clone()
            => (TSelf)MemberwiseClone();

        public IEnumerable<T> GetState<T>()
        {
            var results = new List<T>();

            foreach (var state in StateStack)
            {
                var typedCollection = (state as IEnumerable<T>);

                if (typedCollection != null)
                {
                    results.AddRange(typedCollection);
                }
                else if (state is T)
                {
                    results.Add((T)(state));
                }
            }

            return results;
        }

        public IEnumerable<Object> GetState(Type type)
        {
            if (type == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(type));
            }

            var info = type.GetTypeInfo();

            return GetState<Object>()
                .Where(state =>
                    info.IsAssignableFrom(state.GetType().GetTypeInfo())
                );
        }

        public TSelf PushState(Object state)
        {
            if (state == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(state));
            }

            return (TSelf)this.With(c => c._stateStack, _stateStack.Push(state));
        }

        public TSelf WithProgress(
            IProgress<HarshProvisionerRecord> progress
        )
            => (TSelf)this.With(c => c._progress, progress);

        internal void Report(HarshProvisionerRecord value)
        {
            if (value == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(value));
            }

            if (_progress == null)
            {
                return;
            }

            _progress.Report(value);
        }

        internal ImmutableStack<Object> StateStack => _stateStack;

        Object IHarshCloneable.Clone() => Clone();

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(HarshProvisionerContextBase<>));
    }
}
