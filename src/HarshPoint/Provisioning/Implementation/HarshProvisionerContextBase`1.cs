using System;
using System.Collections.Immutable;

namespace HarshPoint.Provisioning.Implementation
{
    public abstract class HarshProvisionerContextBase<TSelf> :
        HarshProvisionerContextBase, IHarshCloneable
        where TSelf : HarshProvisionerContextBase<TSelf>
    {
        private Boolean _mayDeleteUserData;
        private HarshProvisionerOutputSink _outputSink;
        private IImmutableStack<Object> _stateStack = ImmutableStack<Object>.Empty;

        public sealed override Boolean MayDeleteUserData
            => _mayDeleteUserData;

        public TSelf AllowDeleteUserData()
            => (TSelf)this.With(c => c._mayDeleteUserData, true);

        public TSelf WithOutputSink(HarshProvisionerOutputSink sink)
            => (TSelf)this.With(c => c._outputSink, sink);

        public new TSelf PushState(Object state)
            => (TSelf)PushStateCore(state);

        public virtual TSelf Clone()
            => (TSelf)MemberwiseClone();

        public override void WriteOutput(HarshProvisionerOutput output)
        {
            _outputSink?.WriteOutput(this, output);
        }

        internal sealed override IImmutableStack<Object> StateStack => _stateStack;

        internal sealed override HarshProvisionerContextBase PushStateCore(Object state)
        {
            if (state == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(state));
            }

            return this.With(c => c._stateStack, _stateStack.Push(state));
        }

        Object IHarshCloneable.Clone() => Clone();

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(HarshProvisionerContextBase<>));
    }
}
