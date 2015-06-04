using System;
using System.Collections.Immutable;

namespace HarshPoint.Provisioning.Implementation
{
    public abstract class HarshProvisionerContextBase<TSelf>
        : HarshProvisionerContextBase
        where TSelf : HarshProvisionerContextBase<TSelf>
    {
        private Boolean _mayDeleteUserData;
        private IImmutableStack<Object> _stateStack = ImmutableStack<Object>.Empty;

        public override Boolean MayDeleteUserData => _mayDeleteUserData;

        public override IImmutableStack<Object> StateStack => _stateStack;

        public TSelf AllowDeleteUserData()
        {
            if (MayDeleteUserData)
            {
                return (TSelf)(this);
            }

            return With(c => c._mayDeleteUserData = true);
        }

        public new TSelf PushState(Object state)
        {
            return (TSelf)PushStateCore(state);
        }

        public virtual TSelf Clone()
        {
            return (TSelf)MemberwiseClone();
        }

        protected sealed override HarshProvisionerContextBase PushStateCore(Object state)
        {
            if (state == null)
            {
                throw Error.ArgumentNull(nameof(state));
            }

            return With(c => c._stateStack = _stateStack.Push(state));
        }

        private TSelf With(Action<TSelf> action)
        {
            var result = Clone();
            action(result);
            return result;
        }
    }
}
