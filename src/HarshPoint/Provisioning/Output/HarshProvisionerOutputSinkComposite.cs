using System.Collections.Immutable;

namespace HarshPoint.Provisioning.Output
{
    public sealed class HarshProvisionerOutputSinkComposite : HarshProvisionerOutputSink
    {
        private readonly ImmutableArray<HarshProvisionerOutputSink> _sinks;

        public HarshProvisionerOutputSinkComposite(params HarshProvisionerOutputSink[] sinks)
        {
            if (sinks == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(sinks));
            }

            _sinks = ImmutableArray.CreateRange(sinks);
        }

        protected internal override void WriteOutputCore(HarshProvisionerOutput output)
        {
            foreach (var sink in _sinks)
            {
                sink.WriteOutputCore(output);
            }
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<HarshProvisionerOutputSinkComposite>();
    }
}
